using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PasswordVisibilityToggle : MonoBehaviour
{
    [SerializeField] TMP_InputField passwordLoginField;
    [SerializeField] Button visibilityButton;

    private void Start()
    {
        // Asegúrate de que el campo de contraseña y el botón de visibilidad están asignados
        if (passwordLoginField == null || visibilityButton == null)
        {
            Debug.LogError("Assign the password input field and visibility button in the inspector.");
            return;
        }

        // Configura la acción de clic para el botón de visibilidad
        visibilityButton.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        // Cambia la visibilidad del campo de contraseña entre texto normal y asteriscos
        if (passwordLoginField.contentType == TMP_InputField.ContentType.Password)
        {
            passwordLoginField.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            passwordLoginField.contentType = TMP_InputField.ContentType.Password;
        }

        // Actualiza el campo de entrada para aplicar los cambios
        passwordLoginField.ForceLabelUpdate();
    }
}
