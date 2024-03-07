using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using Firebase.Extensions;
using APIS;
using System.Threading.Tasks;

public class Avatar : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private DatabaseAPI firebase;
    private bool resultUpdateAvatar;
    
    private async void Start(){
        firebase = new DatabaseAPI();
        await firebase.StartUnity(); //esperar hasta que se cumpla la tarea
        auth = firebase.auth;
        db = firebase.db;
    }
    public void SelectWomanAvatar(){
        //El usuario ha iniciado sesión, ahora puedes acceder a su correo electrónico
        string userEmail = auth.CurrentUser.Email;
        // Crea un diccionario con los datos que deseas actualizar
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "Avatar", "Mujer"},
        };

        resultUpdateAvatar = firebase.UpdateData(userEmail, data); 
        
        SceneManager.LoadScene("MainMenu");
        
    }
    public void SelectManAvatar(){
        // El usuario ha iniciado sesión, ahora puedes acceder a su correo electrónico
        string userEmail = auth.CurrentUser.Email;
        // Crea un diccionario con los datos que deseas actualizar
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "Avatar", "Hombre"},
        };
        resultUpdateAvatar = firebase.UpdateData(userEmail, data); 
        
        SceneManager.LoadScene("MainMenu");
        
    }
}
