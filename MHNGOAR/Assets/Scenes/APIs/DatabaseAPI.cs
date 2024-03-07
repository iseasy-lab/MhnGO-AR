using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Database;
using System.Threading.Tasks;


namespace APIS {
    public class DatabaseAPI
    {
        //variables para iniciar firebase
        public DependencyStatus dependencyStatus;
        public FirebaseAuth auth;
        public FirebaseFirestore db;
        public DocumentReference docRef;

        public Task StartUnity(){
            //checa todas las dependencias necesaria para que firebase esté presente en el sistema
            return FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task=>
            {
                dependencyStatus = task.Result;
                if(dependencyStatus == DependencyStatus.Available){
                    //configurar objetos de sesión
                    auth = FirebaseAuth.DefaultInstance;
                    db = FirebaseFirestore.DefaultInstance;
                }else{
                    Debug.LogError("Could not resolve all firebase dependencies:"+dependencyStatus);
                }
            });
        }

        public void RegisterUserData(string nameRegister, string schoolRegister, string emailRegister)
        {
            //array de insignias
            string[] arrayInsignias = new string[]{};
            string[] arrayAcertijosPaleontologia = new string[]{};
            string[] arrayAcertijosVidaSilvestre = new string[]{};
            Dictionary<string, object> user = new Dictionary<string, object>
            {
                { "Nombre", nameRegister},
                { "Escuela", schoolRegister},
                { "Puntos", 0},
                { "Insignias", arrayInsignias},
                { "Avatar", ""},
                {"AcertijosDioramaPaleontologia", arrayAcertijosPaleontologia},
                {"AcertijosDioramaVidaSilvestre", arrayAcertijosVidaSilvestre}
            };

            db.Collection("Usuarios").Document(emailRegister).SetAsync(user).ContinueWith(task => {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("User data registration failed: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    
                    Debug.Log("User data registered successfully");
                }
            });
        }

        public bool UpdateData(string userEmail, Dictionary<string, object> data){
            bool state = false;
            docRef = db.Collection("Usuarios").Document(userEmail);
            // Actualiza el documento con los nuevos datos
            docRef.UpdateAsync(data).ContinueWithOnMainThread(updateTask => {
                if (updateTask.IsCompleted && !updateTask.IsFaulted){
                    // Los datos se han actualizado correctamente
                    Debug.Log("Datos actualizados correctamente.");
                    state = true;
                }else{
                    // Ocurrió un error al actualizar los datos
                    Debug.LogError("Error al actualizar los datos: " + updateTask.Exception);
                    state = false;
                }
            });
            return state;    
        }
    }
}