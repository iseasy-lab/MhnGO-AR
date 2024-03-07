using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Extensions;
using System.Linq;
using System.Threading.Tasks;

public class Insignias : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    [SerializeField] GameObject panelPrefab; // Asigna el prefab del nuevo panel desde el Inspector
    public Transform canvasParticipantes;
    List<object> arrayUser;
    private DocumentReference docRef;
    [SerializeField] TextMeshProUGUI pointsUser;
    public Image imageProfile;
    public Sprite womenProfileImage;
    public Sprite manProfileImage;
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser != null){
            // El usuario está autenticado
            // Inicializa FirebaseFirestore
            db = FirebaseFirestore.DefaultInstance;
            // Llama al método para obtener documentos
            GetInsignias("Usuarios");

        }
    }
    private void GetInsignias(string diorama){
        //obgener las insignias
        CollectionReference collectionRef = db.Collection("Insignia");
       
        collectionRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot snapshot = task.Result;
            //lista que almacena los documentos para mezclar su orden
            List<DocumentSnapshot> documents = new List<DocumentSnapshot>();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    //guardar los documentos en la lista
                    documents.Add(document);
                }
            }
            //verificar que documentos que se deben mostrar, según la dificultad y el usuario.
            GetDocumentsInsignias(documents);
        });
    }

    private void GetDocumentsInsignias(List<DocumentSnapshot> documents)
    {
        //obtener los datos de usuario autenticado
        string userEmail = auth.CurrentUser.Email;
        //documento del usuario autenticado
        docRef = db.Collection("Usuarios").Document(userEmail);
        //obtener datos del documento del usuario
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {

                    //acceder a los atributos del documento del usuario
                    Dictionary<string, object> documentDictionary = snapshot.ToDictionary();
                    pointsUser.text = documentDictionary["Puntos"].ToString();
                    string avatarUser = documentDictionary["Avatar"].ToString();
                    if(avatarUser == "Mujer"){
                        imageProfile.sprite = womenProfileImage;
                    }
                    if(avatarUser=="Hombre"){
                        imageProfile.sprite = manProfileImage;
                    }
                    // Accede al campo array acertijos diorama paleontologia/vidasilvestre del usuario
                    documentDictionary.TryGetValue("Insignias", out object arrayObject);
                    arrayUser = (List<object>)arrayObject;
                    
                    int x = 580;
                    int y = 100;

                    foreach (DocumentSnapshot document in documents){
                        Dictionary<string, object> data = document.ToDictionary();
                        //nombre y puntos del documento
                        string nombre = data["Nombre"].ToString();
                        string descripcion = data["Descripcion"].ToString();
                        //verificar el nivel de dificultad para elegir los acertijos con ese nivel
                        if (arrayUser.Contains(document.Id)){
                            //crear nuevos botones a partir de un botón prefabricado
                            GameObject nuevoPanel = Instantiate(panelPrefab, new Vector3(x, y, 0), Quaternion.identity);
                            nuevoPanel.transform.SetParent(canvasParticipantes.transform); // Asegúrate de tener un Canvas con el nombre "Canvas" en la escena           

                            // Acceder a ambos componentes Text del panel para cambiar sus textos
                            TextMeshProUGUI textoName = nuevoPanel.transform.Find("NameTxt").GetComponent<TextMeshProUGUI>();
                            TextMeshProUGUI textoDescripcion = nuevoPanel.transform.Find("PointsTxt").GetComponent<TextMeshProUGUI>();
                            
                            textoName.text = nombre;
                            textoDescripcion.text = descripcion;
                            
                            //cambiar posición para el nuevo bot+on
                            y = y + 220;
                        }            
                    }
                }
                        
            }            
        });
    }
     public void SignOut(){
        auth.SignOut();
        //activar la escena login
        SceneManager.LoadScene(0);
    }
}
