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
public class MainMenuAcertijos : MonoBehaviour
{
    private FirebaseAuth auth;
    public Image imageProfile;
    public Sprite womenProfileImage;
    public Sprite manProfileImage;
    [SerializeField] Button botonPrefabDioramaAcertijo; // Asigna el prefab del nuevo botón desde el Inspector
    private FirebaseFirestore db;
    [SerializeField] GameObject panelAcertijos;
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser != null){
            //mensaje.text = auth.CurrentUser.DisplayName;
            //acceder a firebase
            db = FirebaseFirestore.DefaultInstance;
            //crear los botones para los acertijos para los dioramas existentes
            HacerBotonAcertijoPrefabricado();
            changeProfileImage();
        }
    }

    public void changeProfileImage(){
        string userEmail = auth.CurrentUser.Email;
        DocumentReference docRef = db.Collection("Usuarios").Document(userEmail);

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    // El documento existe, y puedes acceder al atributo
                    var data = snapshot.ToDictionary();
                    string avatarUser = data["Avatar"].ToString();
                    if(avatarUser == "Mujer"){
                        imageProfile.sprite = womenProfileImage;
                    }
                    if(avatarUser=="Hombre"){
                        imageProfile.sprite = manProfileImage;
                    }
                }
            }
        });
    }

    public void HacerBotonAcertijoPrefabricado(){
        //collection diorama
        CollectionReference collectionRef = db.Collection("Diorama");
        //lista que contiene los documentos de la colección Diorama
        List<DocumentSnapshot> documentosDiorama = new List<DocumentSnapshot>();
        collectionRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                // Manejar el error
                Debug.LogError("Error al obtener documentos: " + task.Exception);
                return;
            }

            // Obtener el resultado de la consulta
            QuerySnapshot snapshot = task.Result;
            int x = 580;
            int y = 700;
            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                //guardar los documentos dioramas
                documentosDiorama.Add(documentSnapshot);
                // Acceder a los datos del documento
                string documentId = documentSnapshot.Id;
                //crear nuevos botones a partir de un botón prefabricado
                Button nuevoBoton = Instantiate(botonPrefabDioramaAcertijo, new Vector3(x, y, 0), Quaternion.identity);
                nuevoBoton.transform.SetParent(GameObject.Find("AreasPanel").transform); // Asegúrate de tener un Canvas con el nombre "Canvas" en la escena           
                TextMeshProUGUI textoBoton = nuevoBoton.GetComponentInChildren<TextMeshProUGUI>();
                textoBoton.text = documentSnapshot.Id.ToUpper();
                Debug.Log(textoBoton.text);
                nuevoBoton.onClick.AddListener(() => OnBotonClic(textoBoton.text));
                y = y + 520;
            }
        });
    }

    public void OnBotonClic(string textoBoton){
        // Almacena el texto en PlayerPrefs (o en un Singleton si es necesario)
        PlayerPrefs.SetString("TextoDelBotonArea", textoBoton);
        // Carga el panel para mostrar los acertijos
        panelAcertijos.SetActive(true);
    }
    
    public void SignOut(){
        auth.SignOut();
        //activar la escena login
        SceneManager.LoadScene(0);
    }
    
}
