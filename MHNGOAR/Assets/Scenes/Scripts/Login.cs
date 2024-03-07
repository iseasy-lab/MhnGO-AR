using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using Firebase.Extensions;
using Firebase.Database;
using APIS;
using Google;

namespace LoginNameSpace{
    public class Login : MonoBehaviour
    {
        private DatabaseAPI firebase;
        public FirebaseAuth auth;
        public FirebaseUser user;
        private FirebaseFirestore db;
        //variables para login
        [SerializeField] TMP_InputField emailLoginField;
        [SerializeField] TMP_InputField passwordLoginField;
        [SerializeField] GameObject prefabOk;
        [SerializeField] GameObject prefabBad;
        private TextMeshProUGUI description;
        private GameObject newPrefab;
        private int x = 550;
        private int y = 1850;
        public Transform canvasMessage;

        string avatar;

        private async void Start(){
            firebase = new DatabaseAPI();
            await firebase.StartUnity(); //esperar hasta que se cumpla la tarea
            auth = firebase.auth;
            db = firebase.db;
            AuthStateChanged(null);
        }

        void AuthStateChanged(System.EventArgs eventArgs){
            //verificar si un usuario se logeo o no
            if(auth.CurrentUser != user){
                bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
                if(!signedIn && user != null){
                    Debug.Log("Signed out " + user.UserId);
                }
                user = auth.CurrentUser;
                if(signedIn){//en caso de que ya haya iniciado sesión
                    //mostrar la siguiente interfaz para elegir el personaje
                    SceneManager.LoadScene("MainMenu");
                    Debug.Log("Signed in" + user.UserId);
                }
            }
        }

        public void LoginUser(){
            StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
        }

        private IEnumerator LoginAsync(string email, string password){
            //inicia sesión
            var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
            //inicializar la tarea
            yield return new WaitUntil(()=>loginTask.IsCompleted);

            if(loginTask.Exception != null){
                Debug.LogError(loginTask.Exception);
                //cuando existe un error al log
                FirebaseException firebaseException = (FirebaseException)loginTask.Exception.GetBaseException();
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string failedMessage = "Login Failed! Because";
                //mensaje prefab de error
                newPrefab = Instantiate(prefabBad, new Vector3(x, y, 0), Quaternion.identity);
                newPrefab.transform.SetParent(canvasMessage.transform);
                description = newPrefab.transform.Find("MesaggeTxt").GetComponent<TextMeshProUGUI>();

                //tipos de error 
                switch (authError){
                    case AuthError.InvalidEmail:
                        failedMessage += "Email incorrecto";
                        description.text = "Email incorrecto";
                        break;
                    case AuthError.WrongPassword:
                        failedMessage += "Contraseña incorrecta";
                        description.text = "Contraseña incorrecta";
                        break;
                    case AuthError.MissingEmail:
                        failedMessage += "Email vacío";
                        description.text = "Email vacío";
                        break;
                    case AuthError.MissingPassword:
                        failedMessage += "Contraseña vacía";
                        description.text = "Contraseña vacía";
                        break;
                    default:
                        failedMessage += "Login Failed";
                        break;
                }
                Debug.Log(failedMessage);
            }else{
                string userEmail = auth.CurrentUser.Email;
                ChangeSceneAvatar(userEmail);
            }
        }

        public void ChangeSceneAvatar(string userEmail){
            DocumentReference docRef = db.Collection("Usuarios").Document(userEmail);
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
                if (task.IsCompleted){
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists){
                        // El documento existe, y puedes acceder al atributo avatar
                        //diccionario 
                        var data = snapshot.ToDictionary();
                        avatar = data["Avatar"].ToString();
                        //cuando ya se eligió un avatar
                        if (avatar == ""){
                            // Cambia a la escena deseada en el hilo principal
                            SceneManager.LoadScene("SelectAvatar");                        
                        }else{        
                            SceneManager.LoadScene("MainMenu");            
                        }
                    }else{
                        newPrefab = Instantiate(prefabBad, new Vector3(x, y, 0), Quaternion.identity);
                        newPrefab.transform.SetParent(canvasMessage.transform);
                        description = newPrefab.transform.Find("MesaggeTxt").GetComponent<TextMeshProUGUI>();
                        description.text = "El usario no está registrado";
                        GoogleSignIn.DefaultInstance.SignOut();
                    }
                }
            });
        }
    }
}