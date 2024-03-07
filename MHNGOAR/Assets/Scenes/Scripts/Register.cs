using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;
using Firebase.Extensions;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using APIS;

public class Register : MonoBehaviour
{
    public FirebaseAuth auth;
    private DatabaseAPI firebase;
    [SerializeField] TMP_InputField nameRegisterField;
    [SerializeField] TMP_InputField schoolRegisterField;
    [SerializeField] TMP_InputField emailRegisterField;
    [SerializeField] TMP_InputField passwordRegisterField;
    [SerializeField] TMP_InputField confirmPasswordRegisterField;
    private FirebaseFirestore db;
    [SerializeField] GameObject prefabOk;
    [SerializeField] GameObject prefabBad;
    private GameObject newPrefab;
    private TextMeshProUGUI description;
    private int x = 550;
    private int y = 1850;
    public Transform canvasMessage;
    // Start is called before the first frame update
    async void Start()
    {
        firebase = new DatabaseAPI();
        await firebase.StartUnity(); //esperar hasta que se cumpla la tarea
        auth = firebase.auth;
        db = firebase.db;
    }

    public void RegisterAuthentication(){
        StartCoroutine(RegisterAsync(nameRegisterField.text,emailRegisterField.text, passwordRegisterField.text, confirmPasswordRegisterField.text, schoolRegisterField.text));
    }

    private IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword, string school){
        newPrefab = Instantiate(prefabBad, new Vector3(x, y, 0), Quaternion.identity);
        newPrefab.transform.SetParent(canvasMessage.transform);
        description = newPrefab.transform.Find("MesaggeTxt").GetComponent<TextMeshProUGUI>();
        //verificar que se ingreso todos los valores
        if (name == ""){
            Debug.LogError("Nombre de usuario vacío");
            description.text = "Nombre de usuario vacío";
        }else if (email == ""){
            Debug.LogError("Email vacío");
            description.text = "Email vacío";
        }else if (school == ""){
            Debug.LogError("Escuela vacía");
            description.text = "Escuela vacía";
        }else if (passwordRegisterField.text != confirmPasswordRegisterField.text){
            Debug.LogError("Contraseña no coincide");
            description.text = "Contraseña no coincide";
        }else{
            //crear un usuario
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(()=>registerTask.IsCompleted);
            //cuando el registro falló
            if(registerTask.Exception != null){
                Debug.LogError(registerTask.Exception);
                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;
                
                //causas de falla
                switch(authError){
                    case AuthError.InvalidEmail:
                        description.text = "Email incorrecto";
                        break;
                    case AuthError.WrongPassword:
                        description.text = "Contraseña incorrecta";
                        break;
                    case AuthError.MissingEmail:
                        description.text = "Email vacío";
                        break;
                    case AuthError.MissingPassword:
                        description.text = "Password vacío";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        description.text = "El email ya está en uso";
                        break;
                    default:
                        description.text = "Registro fallido";
                        break;
                }
                Debug.Log("Falla al crear");
            }else{
                
                AuthResult authResult = registerTask.Result;
                FirebaseUser user = authResult.User;
                
                //cuando el registro es exitoso
                Firebase.Auth.AuthResult result = registerTask.Result;
                UserProfile userProfile = new UserProfile{DisplayName = name};
                var updateProfileTask = result.User.UpdateUserProfileAsync(userProfile);
                yield return new WaitUntil(()=>updateProfileTask.IsCompleted);
                //registrar al usuario en el firebase
                firebase.RegisterUserData(nameRegisterField.text,schoolRegisterField.text,emailRegisterField.text);
                auth.SignOut();
                newPrefab = Instantiate(prefabOk, new Vector3(x, y, 0), Quaternion.identity);
                newPrefab.transform.SetParent(canvasMessage.transform);
                description = newPrefab.transform.Find("MesaggeTxt").GetComponent<TextMeshProUGUI>();
                description.text = "REGISTRO EXITOSO " + nameRegisterField.text;
            }
        }
    }
}
