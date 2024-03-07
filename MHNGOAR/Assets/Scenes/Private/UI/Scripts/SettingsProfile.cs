using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine.SceneManagement;
using Firebase.Extensions;
using APIS;

public class SettingsProfile : MonoBehaviour
{
    //variables para actualizar
    [SerializeField] TMP_InputField nameUpdate;
    [SerializeField] TMP_InputField schoolUpdate;
    public Image imageProfile;
    public Sprite womenProfileImage;
    public Sprite manProfileImage;
    private FirebaseFirestore db;
    [SerializeField] TextMeshProUGUI mensaje;
    private FirebaseAuth auth;
    [SerializeField] GameObject prefabOk;
    [SerializeField] GameObject prefabBad;
    private GameObject newPrefab;
    private TextMeshProUGUI description;
    private int x = 550;
    private int y = 1200;
    public Transform canvasMensaje;
    private bool resultUpdate;
    private DatabaseAPI firebase;
    
    // Start is called before the first frame update
    async void Start()
    {
        firebase = new DatabaseAPI();
        await firebase.StartUnity(); //esperar hasta que se cumpla la tarea
        auth = firebase.auth;
        db = firebase.db;
        ChargueData();
        
    }



    private void ChargueData(){
        //obtener los datos de usuario autenticado
        string userEmail = auth.CurrentUser.Email;
        db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("Usuarios").Document(userEmail);

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    // El documento existe, y puedes acceder al atributo
                    var data = snapshot.ToDictionary();
                    nameUpdate.text = data["Nombre"].ToString();
                    schoolUpdate.text = data["Escuela"].ToString();
                    //atributo imagen avatar
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

    public void UpdateData(){
        newPrefab = Instantiate(prefabBad, new Vector3(x, y, 0), Quaternion.identity);
        newPrefab.transform.SetParent(canvasMensaje.transform);
        description = newPrefab.transform.Find("MesaggeTxt").GetComponent<TextMeshProUGUI>();
            
        //verificar que se ingreso todos los valores
        if (nameUpdate.text == ""){
            description.text = "Nombre de usuario vacío";
        }else if (schoolUpdate.text == ""){
            description.text = "Escuela vacía";
        }else{
            //obtener los datos de usuario autenticado
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            string userEmail = auth.CurrentUser.Email;
            //actualiza el email de autenticación
            db = FirebaseFirestore.DefaultInstance;
            //actualizar el nombre del documento con el nuevo email
            //referencia al documento email anterior
            DocumentReference docRef = db.Collection("Usuarios").Document(userEmail);
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
                if (task.IsCompleted){
                    // Crea un diccionario con los datos que deseas actualizar del nuevo documento -email
                    Dictionary<string, object> newData = new Dictionary<string, object>{
                        { "Nombre", nameUpdate.text },
                        { "Escuela", schoolUpdate.text }
                    };
                    resultUpdate = firebase.UpdateData(userEmail, newData); 
                    
                    newPrefab = Instantiate(prefabOk, new Vector3(x, y, 0), Quaternion.identity);
                    newPrefab.transform.SetParent(canvasMensaje.transform);
                    description = newPrefab.transform.Find("MesaggeTxt").GetComponent<TextMeshProUGUI>();
                    description.text = "Datos Actualizados";
                }
            });
        }
    }
}
