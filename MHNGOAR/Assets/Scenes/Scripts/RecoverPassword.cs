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

public class RecoverPassword: MonoBehaviour
{
    private DatabaseAPI firebase;
    public FirebaseAuth auth;
    private FirebaseFirestore db;
    [SerializeField] TMP_InputField email;
    [SerializeField] GameObject prefabOk;
    [SerializeField] GameObject prefabBad;
    private TextMeshProUGUI description;
    private GameObject newPrefab;
    private int x = 550;
    private int y = 1750;
    public Transform canvasMessage;

    private async void Start(){
        firebase = new DatabaseAPI();
        await firebase.StartUnity(); //esperar hasta que se cumpla la tarea
        auth = firebase.auth;
        db = firebase.db;
    }

    public void Recover(){
        newPrefab = Instantiate(prefabBad, new Vector3(x, y, 0), Quaternion.identity);
        newPrefab.transform.SetParent(canvasMessage.transform);
        description = newPrefab.transform.Find("MesaggeTxt").GetComponent<TextMeshProUGUI>();
                                
        if (String.IsNullOrEmpty(email.text) == false){
            db = FirebaseFirestore.DefaultInstance;
                
            DocumentReference docRef = db.Collection("Usuarios").Document(email.text);
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
                if (task.IsCompleted){
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists){
                        auth.SendPasswordResetEmailAsync(email.text).ContinueWithOnMainThread(task =>
                        {
                            if (task.IsCanceled)
                            {
                                description.text = "Vuelve a intentarlo";
                                Debug.LogError("Envío de correo de restablecimiento de contraseña cancelado.");
                            }
                            else if (task.IsFaulted)
                            {
                                description.text = "Vuelve a intentarlo";
                                Debug.LogError("Error al enviar el correo de restablecimiento de contraseña: " + task.Exception);
                            }
                            else if (task.IsCompleted)
                            {
                                newPrefab = Instantiate(prefabOk, new Vector3(x, y, 0), Quaternion.identity);
                                newPrefab.transform.SetParent(canvasMessage.transform);
                                description = newPrefab.transform.Find("MesaggeTxt").GetComponent<TextMeshProUGUI>();
                                description.text = "Correo enviado exitosamente";
                                Debug.Log("Correo de restablecimiento de contraseña enviado exitosamente. Revisa tu correo electrónico.");
                            }
                        });
                    }
                    else{
                        description.text = "El usario no está registrado";
                    }
                }
            });
        }else{
            description.text = "Ingresar un correo electrónico";
        }
    }
}
