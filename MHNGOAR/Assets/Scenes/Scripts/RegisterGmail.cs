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
using APIS;

public class RegisterGmail : MonoBehaviour
{
    private DatabaseAPI firebase;
    public FirebaseAuth auth;
    public DependencyStatus dependencyStatus;
    public string webClientId = "550181900371-gl1sn55givi4prkslbdtu2si91a44vib.apps.googleusercontent.com";
    private FirebaseFirestore db;
    private GoogleSignInConfiguration configuration;
    [SerializeField] GameObject prefabOk;
    [SerializeField] GameObject prefabBad;
    public Transform canvasMensaje;

    private DocumentReference docRef;
    private GameObject newPrefab;
    private Firebase.Auth.FirebaseUser usuario;
    // Defer the configuration creation until Awake so the web Client ID
    async void Awake() {
        configuration = new GoogleSignInConfiguration {
            WebClientId = webClientId,
            RequestIdToken = true
        };
        firebase = new DatabaseAPI();
        await firebase.StartUnity(); //esperar hasta que se cumpla la tarea
        auth = firebase.auth;
        db = firebase.db;
        //auth = FirebaseAuth.DefaultInstance;
        //db = FirebaseFirestore.DefaultInstance;
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
        } else if(task.IsCanceled) {
            Debug.Log("Canceled");
        } else  {
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
                //datos del usuario autenticado con gmail
                usuario = taskUserSign.Result;
            
                //validar existencia del usuario en firestore
                ValidarUser(usuario.DisplayName, usuario.Email);
                //cerrar sesión en gmail
                GoogleSignIn.DefaultInstance.SignOut();
            });
        }
    }

    public void ValidarUser(string userDisplayName, string userEmail){
        // Obtiene la referencia de la colección
        CollectionReference colRef = db.Collection("Usuarios");

        // Obtiene todos los documentos de la colección
        colRef.GetSnapshotAsync().ContinueWithOnMainThread(queryTask => {
           
            if (queryTask.IsCanceled || queryTask.IsFaulted){
                Debug.LogError("Error al obtener documentos de la colección: " + queryTask.Exception);
                return;
            }

            QuerySnapshot querySnapshot = queryTask.Result;

            // Verifica si el documento específico existe en la colección
            bool findDocument = false;

            foreach (var documento in querySnapshot.Documents){
                if (documento.Id.Equals(userEmail)){
                      
                    findDocument = true;
                    break;
                }
            }

            newPrefab = Instantiate(prefabOk, new Vector3(550, 1850, 0), Quaternion.identity);
            newPrefab.transform.SetParent(canvasMensaje.transform);
            TextMeshProUGUI description = newPrefab.transform.Find("MesaggeTxt").GetComponent<TextMeshProUGUI>();
                
            if (findDocument){
                description.text = "El usuario ya está registrado";
            }else{
                //el usuario va a ser registrado
                firebase.RegisterUserData(userDisplayName, "", userEmail);
                description.text = "Registro existoso";
            }
        });
    }
}
