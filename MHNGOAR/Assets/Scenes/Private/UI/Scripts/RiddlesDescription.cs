using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using APIS;

public class RiddlesDescription : MonoBehaviour
{
    private DatabaseAPI firebase;
    [SerializeField] TextMeshProUGUI nameArea;
    [SerializeField] TextMeshProUGUI riddleStatement;
    [SerializeField] TextMeshProUGUI riddleDificult;
    [SerializeField] TextMeshProUGUI informationComplement;
    [SerializeField] Button botonPrefab; // Asigna el prefab del nuevo botón desde el Inspector
    List<object> arrayuser;
    long puntos;
    public Image originalImage;
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private DocumentReference docRef;
    [SerializeField] TextMeshProUGUI pointsUser; //mostrar los puntos en la interfaz
    public Transform canvasAnswerRiddles;
    [SerializeField] GameObject panelRiddles;
    [SerializeField] GameObject panelEndRiddles;
    [SerializeField] GameObject prefabOk;
    [SerializeField] GameObject prefabBad;
    private GameObject newPrefab;
    private TextMeshProUGUI description;
    private int x = 550;
    private int y = 1480;
    public float visibleDuration = 2f; // Duración en segundos que se muestra el canvas
    public Transform canvasMessage;
    string nameArray; //dioarama que se va a resolver - acertijos 
    private string userEmail;
    private bool resultUpdate;
    // Start is called before the first frame update
    void OnEnable()
    {
        //limpiar el canvas con los acertijos
        // Verificamos si la referencia al canvas no es nula
        if (canvasAnswerRiddles != null)
        {
            // Eliminamos todos los objetos hijos del canvas
            foreach (Transform hijo in canvasAnswerRiddles.transform)
            {
                Destroy(hijo.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("La referencia al Canvas es nula. Asegúrate de asignar el Canvas en el Inspector.");
        }
        //autenticar con firebase
        auth = FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser != null){
            db = FirebaseFirestore.DefaultInstance;
            
            nameArea.text = PlayerPrefs.GetString("TextoDelBotonArea");
            //mostrar los acertijos y sus respuestas 
            GetAcertijos(nameArea.text);
        }

    }
    private void GetAcertijos(string diorama){
        CollectionReference collectionRef = db.Collection("Acertijo");
        string nuevoValorDiorama = diorama.Replace("\n", "").Replace("\r", ""); // Elimina saltos de línea
        
        // Realizar la consulta para obtener documentos con diorama.
        Query query = collectionRef.WhereEqualTo("Diorama", nuevoValorDiorama);
        query.GetSnapshotAsync().ContinueWithOnMainThread(task =>
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
            seleccionarAcertijos(documents, nuevoValorDiorama);
        });
    }

    public void seleccionarAcertijos(List<DocumentSnapshot> documents, string diorama){
        nameArray = "";
        //obtener el array de acertijos según el diorama
        if (diorama == "PALEONTOLOGÍA"){
            nameArray = "AcertijosDioramaPaleontologia";
        }else{
            if(diorama == "VIDA SILVESTRE"){
                nameArray = "AcertijosDioramaVidaSilvestre";
            }
        }
        //obtener los datos de usuario autenticado
        userEmail = auth.CurrentUser.Email;
        //documento del usuario autenticado
        docRef = db.Collection("Usuarios").Document(userEmail);
        //obtener datos del documento del usuario
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    // El documento existe, y puedes acceder al atributo puntos
                    var data = snapshot.ToDictionary();
                    pointsUser.text = data["Puntos"].ToString();
                    puntos = (long)data["Puntos"];

                    //acceder a los atributos del documento del usuario
                    Dictionary<string, object> documentDictionary = snapshot.ToDictionary();
                    // Accede al campo array acertijos diorama paleontologia/vidasilvestre del usuario
                    documentDictionary.TryGetValue(nameArray, out object arrayObject);
                    arrayuser = (List<object>)arrayObject;
                    //acertijos filtrados que se mostrarán al usuario
                    List<DocumentSnapshot> documentosElegidos = new List<DocumentSnapshot>();
                    if (arrayuser != null)
                    {
                        //recorre cada elemento de la matriz
                        switch(arrayuser.Count){
                            case 0:
                                CambiarColorDeImagenDirecto(Color.green);
                                //selecciona todos los acertijos con nivel bajo
                                documentosElegidos = seleccionarAcertijosConDificultad(documents, "Bajo");
                                mezclarDocumentos(documentosElegidos);
                                break;
                            case <3:
                                CambiarColorDeImagenDirecto(Color.green);
                                //selecciona todos los acertijos con nivel bajo
                                documentosElegidos = seleccionarAcertijosConDificultad(documents, "Bajo");
                                //selecciona todos los acertijos que aún no ha jugado
                                documentosElegidos = seleccionarAcertijosNoJugados(documentosElegidos);
                                mezclarDocumentos(documentosElegidos);
                                break;
                            case 3:
                                CambiarColorDeImagenDirecto(Color.yellow);
                                //selecciona todos los acertijos con nivel medio
                                documentosElegidos = seleccionarAcertijosConDificultad(documents, "Medio");
                                mezclarDocumentos(documentosElegidos);
                                
                                break;
                            case >3 and <6:
                                CambiarColorDeImagenDirecto(Color.yellow);
                                //selecciona todos los acertijos con nivel medio
                                documentosElegidos = seleccionarAcertijosConDificultad(documents, "Medio");
                                //selecciona todos los acertijos que aún no ha jugado
                                documentosElegidos = seleccionarAcertijosNoJugados(documentosElegidos);
                                mezclarDocumentos(documentosElegidos);
                                break;
                            case 6:
                                CambiarColorDeImagenDirecto(Color.red);
                                //selecciona todos los acertijos con nivel alto
                                documentosElegidos = seleccionarAcertijosConDificultad(documents, "Alta");
                                mezclarDocumentos(documentosElegidos);
                                
                                break;
                            case >6 and <9:
                                CambiarColorDeImagenDirecto(Color.red);
                                //selecciona todos los acertijos con nivel alto
                                documentosElegidos = seleccionarAcertijosConDificultad(documents, "Alta");
                                //selecciona todos los acertijos que aún no ha jugado
                                documentosElegidos = seleccionarAcertijosNoJugados(documentosElegidos);
                                mezclarDocumentos(documentosElegidos);
                                break;
                            case 9:
                                //mostrar la escena donde indica que ya se resolvieron todos los acertijos
                                panelRiddles.SetActive(false);
                                panelEndRiddles.SetActive(true);
                                break;
                            default:
                                break;

                        }
                    }
                    else
                    {
                        Debug.LogError("La matriz está vacía o no se pudo recuperar correctamente.");
                    } 
                }
            }
        });
    }

    void CambiarColorDeImagenDirecto(Color nuevoColor)
    {
        // Verificar que se haya asignado la imagen desde el Inspector
        if (originalImage != null)
        {
            // Cambiar el color del componente Image
            originalImage.color = nuevoColor;
        }
        else
        {
            Debug.LogError("La imagen no ha sido asignada desde el Inspector.");
        }
    }
    public List<DocumentSnapshot> seleccionarAcertijosConDificultad(List<DocumentSnapshot> documents, string dificultad){
        //nueva lista de documentos con cierta dificultad y de cierta área
        List<DocumentSnapshot> newDocuments = new List<DocumentSnapshot>();
        
        foreach (DocumentSnapshot document in documents){
            var data = document.ToDictionary();
            //verificar el nivel de dificultad para elegir los acertijos con ese nivel
            if(data["Dificultad"].ToString() == dificultad){
                newDocuments.Add(document);
            }            
        }
        return newDocuments;
    }

    public List<DocumentSnapshot> seleccionarAcertijosNoJugados(List<DocumentSnapshot> documents){
        //nueva lista de documentos con cierta dificultad y de cierta área y que aún no ha resuelto el usuario
        List<DocumentSnapshot> newDocuments = new List<DocumentSnapshot>();
        
        foreach (DocumentSnapshot document in documents){
            var data = document.ToDictionary();
            //verificar el nivel de dificultad para elegir los acertijos con ese nivel
            if (!arrayuser.Contains(document.Id)){
                newDocuments.Add(document);
            }            
        }
        return newDocuments;
    }

    public void mezclarDocumentos(List<DocumentSnapshot> documents)
    {
        
        System.Random rng = new System.Random();
        int n = documents.Count;
        //mezclar los documentos de una lista de documentos
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            DocumentSnapshot value = documents[k];
            documents[k] = documents[n];
            documents[n] = value;
        }
        mostrarDocumento(documents[0]);
        
    }
    public void mostrarDocumento(DocumentSnapshot document1){
        // El documento existe, y puedes acceder al atributo
        var data = document1.ToDictionary();
        riddleStatement.text = data["Enunciado"].ToString();
        //mostrar la dificultad del acertijo
        riddleDificult.text = data["Dificultad"].ToString();
        //mostrar la información complementaria que ayudará a resolver el acertijo
        informationComplement.text = data["InformacionComplementaria"].ToString();
        //obtener la respuesta correcta
        string respuestaCorrectaAcertijo = data["RespuestaCorrecta"].ToString();
        //obtener los puntos del acertijo
        long puntosAcertijo = (long)data["Puntos"];
        //obtener la matriz con las respuestas del documento
        data.TryGetValue("Respuestas", out object answersList);
        List<object> answers = (List<object>)answersList;
        //mezclar el orden de los valores en la matriz
        System.Random rng = new System.Random();
        int n = answers.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var value = answers[k];
            answers[k] = answers[n];
            answers[n] = value;
        }
        //mostrar las respuestas y dibujar botones
        n = 0;
        //ubicación de los primeros botónes
        int x = 300;
        int y = 980;

        while (n < answers.Count)
        {
            //posición de los botones
            if (n == 2){
                y = 850;
                x = 300;
            } 
            //crear nuevos botones a partir de un botón prefabricado
            Button nuevoBoton = Instantiate(botonPrefab, new Vector3(x, y, 0), Quaternion.identity);
            nuevoBoton.transform.SetParent(canvasAnswerRiddles.transform); // Asegúrate de tener un Canvas con el nombre "Canvas" en la escena

            x = 750;
            n++;
        }  

        //Asignar una respuesta a los botones creados previamente
        n = 0;
        // Encuentra todos los botones en la escena
        Button[] botones = FindObjectsOfType<Button>();
        foreach (Button boton in botones)
        {
            // Verifica si el botón es del tipo btnPrefab
            if (boton.name == "PrefabBtnRiddle(Clone)")
            {
                // Accede al componente TextMeshPro del botón
                TextMeshProUGUI textoBoton = boton.GetComponentInChildren<TextMeshProUGUI>();

                // Verifica si se encontró el componente Text
                if (textoBoton != null)
                {
                    // Cambia el texto del botón
                    textoBoton.text = answers[n].ToString();;
                }
                else
                {
                    Debug.LogError("No se encontró el componente Text en el botón.");
                }
                
                //cuando se da clic en un botón
                boton.onClick.AddListener(() => onBotonClic(boton, respuestaCorrectaAcertijo, puntosAcertijo, document1));
            }
            n++;
        }
    }
    
    private void onBotonClic(Button botonClic, string respuestaCorrectaAcertijo, long puntosAcertijo, DocumentSnapshot document1){
        //validar si la respuesta es correcta y dar puntos al usuario
        // Accede al componente TextMeshPro del botón
        TextMeshProUGUI textoBoton = botonClic.GetComponentInChildren<TextMeshProUGUI>();
        string respuestaBoton = textoBoton.text;
        if (respuestaBoton == respuestaCorrectaAcertijo){
            Debug.Log("La respuesta es correcta");
            //obtener puntos cuando se responde correctamente el acertijo
            getNewPoints(puntosAcertijo, document1);
            //Asignar una insignia si es cumple con los requisitos
            GetBadgeFirstRiddle();
            GetBadgePaleontologyExpert();
            GetBadgeWildLifeExpert();
            //mostrar mesaje de respuesta correcta
            newPrefab = Instantiate(prefabOk, new Vector3(x, y, 0), Quaternion.identity);
            newPrefab.transform.SetParent(canvasMessage.transform);
            description = newPrefab.transform.Find("MesaggeTxt").GetComponent<TextMeshProUGUI>();
            description.text = "Respuesta Correcta";
            Destroy(newPrefab, visibleDuration);
            //vuelver a abrir la escena considerando los acertijos resueltos para el diorama respectivo
            //si ya resolvio todos los acertijos debe mostrar el escenario donde diga que todos los acertijos fueron resueltos
            panelRiddles.SetActive(false);
            panelRiddles.SetActive(true);
        }else{
            Debug.Log("La respuesta es incorrecta");
            newPrefab = Instantiate(prefabBad, new Vector3(x, y, 0), Quaternion.identity);
            newPrefab.transform.SetParent(canvasMessage.transform);
            description = newPrefab.transform.Find("MesaggeTxt").GetComponent<TextMeshProUGUI>();
            description.text = "Respuesta Incorrecta";
            Destroy(newPrefab, visibleDuration);
        }
    }

    public void getNewPoints(long puntosAcertijo, DocumentSnapshot document1){
        
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted){
                //agregar el acertijo a la lista de acertijos resueltos por el usuario
                arrayuser.Add(document1.Id);
                // Crea un diccionario con los puntos a actualizar
                //actualizar la cantidad de acertijos resueltos
                Dictionary<string, object> newData = new Dictionary<string, object>{
                    { "Puntos", puntos +  puntosAcertijo},
                    {nameArray, arrayuser}
                };
                resultUpdate = firebase.UpdateData(userEmail, newData); 
            }
        });
    }
    //obtener la primera insignia por resolver el primer acertijo
    public void GetBadgeFirstRiddle(){
        //obtener datos del documento del usuario
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted){
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    if (arrayuser.Count == 1){
                        //acceder a los atributos del documento del usuario
                        Dictionary<string, object> documentDictionary = snapshot.ToDictionary();
                        // Accede al campo array Insignias del usuario
                        documentDictionary.TryGetValue("Insignias", out object arrayObject);
                        //lista que almacena almacena las insignias del usuario
                        List<object> arrayInsigniasUsuario = (List<object>)arrayObject;
                        //acertijos filtrados que se mostrarán al usuario
                        arrayInsigniasUsuario.Add("Insignia1");
                        //actualizar las insignias ganadas
                        Dictionary<string, object> newData = new Dictionary<string, object>{
                            {"Insigias", arrayInsigniasUsuario}
                        };
                        resultUpdate = firebase.UpdateData(userEmail, newData);
                    }
                }
            }
        });
    }

    //obtener la insignia por resolver todos los acertijos de paleontología
    public void GetBadgePaleontologyExpert(){
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted){
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    if (arrayuser.Count == 9 && nameArray=="AcertijosDioramaPaleontologia"){
                        //acceder a los atributos del documento del usuario
                        Dictionary<string, object> documentDictionary = snapshot.ToDictionary();
                        // Accede al campo array Insignias del usuario
                        documentDictionary.TryGetValue("Insignias", out object arrayObject);
                        //lista que almacena almacena las insignias del usuario
                        List<object> arrayInsigniasUsuario = (List<object>)arrayObject;
                        //acertijos filtrados que se mostrarán al usuario
                        arrayInsigniasUsuario.Add("Insignia2");
                        //actualizar las insignias ganadas
                        Dictionary<string, object> newData = new Dictionary<string, object>{
                            {"Insigias", arrayInsigniasUsuario}
                        };
                        resultUpdate = firebase.UpdateData(userEmail, newData);
                    }
                }
            }
        });
    }

    //obtener la insignia por resolver todos los acertijos de vida silvestre
    public void GetBadgeWildLifeExpert(){
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted){
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    if (arrayuser.Count == 9 && nameArray=="AcertijosDioramaVidaSilvestre"){
                        //acceder a los atributos del documento del usuario
                        Dictionary<string, object> documentDictionary = snapshot.ToDictionary();
                        // Accede al campo array Insignias del usuario
                        documentDictionary.TryGetValue("Insignias", out object arrayObject);
                        //lista que almacena almacena las insignias del usuario
                        List<object> arrayInsigniasUsuario = (List<object>)arrayObject;
                        //acertijos filtrados que se mostrarán al usuario
                        arrayInsigniasUsuario.Add("Insignia3");
                        //actualizar las insignias ganadas
                        Dictionary<string, object> newData = new Dictionary<string, object>{
                            {"Insigias", arrayInsigniasUsuario}
                        };
                        resultUpdate = firebase.UpdateData(userEmail, newData);
                    }
                }
            }
        });
    }
}
