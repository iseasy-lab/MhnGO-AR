using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Extensions;
using LoginNameSpace;

namespace LoginGmail{
    public class LoginGmail : MonoBehaviour
    {
        private Login login;
        public FirebaseAuth auth;
        public string webClientId = "550181900371-gl1sn55givi4prkslbdtu2si91a44vib.apps.googleusercontent.com";
        private FirebaseFirestore db;
        private GoogleSignInConfiguration configuration;
        private Firebase.Auth.FirebaseUser usuario;
        /*[SerializeField] GameObject prefabOk;
        [SerializeField] GameObject prefabBad;
        private TextMeshProUGUI description;
        private GameObject newPrefab;
        private int x = 550;
        private int y = 1850;
        public Transform canvasMensaje;
        string avatar;*/
        // Defer the configuration creation until Awake so the web Client ID
        void Awake() {
            configuration = new GoogleSignInConfiguration {
                WebClientId = webClientId,
                RequestIdToken = true
            };
            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseFirestore.DefaultInstance;
        }

        public void OnSignIn() {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
            GoogleSignIn.Configuration.RequestEmail = true;
            //Inicia sesión gmail
            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
        }

        public void OnAuthenticationFinished(Task<GoogleSignInUser> task) {
        
            if (task.IsFaulted) {
                //atrapar errores al querer conectarse con gmail
                using (IEnumerator<System.Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator()) {
                    if (enumerator.MoveNext()) {
                        GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                        Debug.Log("Got Error: " + error.Status + error.Message);
                    } else {
                        Debug.Log("Got Unexpected Exception?!?" + task.Exception);
            
                    } 
                }
            }else if(task.IsCanceled) {
                Debug.Log("Canceled");
            
            }else{
        
                Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
                //ingresar sesión con la cuenta de gmail
                auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(taskUserSign => {
            
                    if (taskUserSign.IsCanceled) {
                        Debug.Log("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                        return;
                    }
                    if (taskUserSign.IsFaulted) {
                        Debug.Log("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + taskUserSign.Exception);
                        return;
                    }
                    usuario = taskUserSign.Result;
                    login = new Login();
                    login.ChangeSceneAvatar(usuario.Email);
                });
            }
        }
    }
}