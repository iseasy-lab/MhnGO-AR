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

public class Ranking : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    [SerializeField] TextMeshProUGUI puntos1erLugar;
    [SerializeField] TextMeshProUGUI nombre1erLugar;
    [SerializeField] TextMeshProUGUI puntos2doLugar;
    [SerializeField] TextMeshProUGUI nombre2doLugar;
    [SerializeField] TextMeshProUGUI puntos3erLugar;
    [SerializeField] TextMeshProUGUI nombre3erLugar;
    [SerializeField] TextMeshProUGUI pointsUser;
    [SerializeField] GameObject panelPrefab; // Asigna el prefab del nuevo panel desde el Inspector
    public Transform canvasParticipantes;
    public Image imageProfile;
    public Sprite womenProfileImage;
    public Sprite manProfileImage;

    // Start is called before the first frame update
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser != null){
            // El usuario está autenticado
            // Inicializa FirebaseFirestore
            db = FirebaseFirestore.DefaultInstance;
            // Llama al método para obtener documentos
            GetDocumentsSortedByPoints("Usuarios");

        }
    }
    private async void GetDocumentsSortedByPoints(string collectionName)
    {
        CollectionReference collectionRef = db.Collection(collectionName);

        Query query = collectionRef.OrderBy("Puntos");
        QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

        List<DocumentSnapshot> documents = querySnapshot.Documents.ToList();
        documents.Reverse(); // Invierte el orden de los documentos
        int posicion = 0;
        //inicializar participantes puntos acumulados
        //puntosAcumulados.text = "";
        foreach (DocumentSnapshot documentSnapshot in documents)
        {
            if (documentSnapshot.Exists)
            {
                //aumentar la posición de los jugadores
                posicion += 1;
                //datos del documento
                Dictionary<string, object> data = documentSnapshot.ToDictionary();
                if(documentSnapshot.Id == auth.CurrentUser.Email){
                    pointsUser.text = data["Puntos"].ToString();
                    string avatarUser = data["Avatar"].ToString();
                    if(avatarUser == "Mujer"){
                        imageProfile.sprite = womenProfileImage;
                    }
                    if(avatarUser=="Hombre"){
                        imageProfile.sprite = manProfileImage;
                    }
                }
                //nombre y puntos del documento
                string nombre = data["Nombre"].ToString();
                long puntos = (long)data["Puntos"];

                //posición de los bótones prefab 
                int x = 580;
                int y = 100;
                switch(posicion){
                    case 1:
                        // Actualiza el objeto de texto con los valores obtenidos
                        puntos1erLugar.text = System.Convert.ToString(puntos);
                        nombre1erLugar.text = nombre;
                        break;
                    case 2:
                        puntos2doLugar.text = System.Convert.ToString(puntos);
                        nombre2doLugar.text = nombre;
                        break;
                    case 3:
                        puntos3erLugar.text = System.Convert.ToString(puntos);
                        nombre3erLugar.text = nombre;
                        break;
                    case >3:

                        //crear nuevos botones a partir de un botón prefabricado
                        GameObject nuevoPanel = Instantiate(panelPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        nuevoPanel.transform.SetParent(canvasParticipantes.transform); // Asegúrate de tener un Canvas con el nombre "Canvas" en la escena           

                        // Acceder a ambos componentes Text del panel para cambiar sus textos
                        TextMeshProUGUI textoName = nuevoPanel.transform.Find("NameTxt").GetComponent<TextMeshProUGUI>();
                        TextMeshProUGUI textoPoints = nuevoPanel.transform.Find("PointsTxt").GetComponent<TextMeshProUGUI>();
                        
                        textoName.text = nombre;
                        textoPoints.text = System.Convert.ToString(puntos);
                        
                        //cambiar posición para el nuevo bot+on
                        y = y + 220;
                        break;
                    default:
                        break;
                }
            }
        }
    }
     public void SignOut(){
        auth.SignOut();
        //activar la escena login
        SceneManager.LoadScene(0);
    }
}
