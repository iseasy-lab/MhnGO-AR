using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanelMovenment : MonoBehaviour
{
    public GameObject InfoOrigPos;
    public GameObject InfoActivePos;
    public GameObject InfoPanel;

    public bool Info_Menu_Panel, Info_Menu_Panel_Back;

    public float moveSpeed;
    public float tempMenuPos;

    // Start is called before the first frame update
    void Start()
    {
        InfoPanel.transform.position = InfoOrigPos.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (Info_Menu_Panel)
        {
            InfoPanel.transform.position = Vector3.Lerp(InfoPanel.transform.position, InfoActivePos.transform.position, moveSpeed * Time.deltaTime);

            if (InfoPanel.transform.localPosition.x == tempMenuPos)
            {
                Info_Menu_Panel = false;
                InfoPanel.transform.position = InfoActivePos.transform.position;
                tempMenuPos = -999999999.99f;
            }
            if (Info_Menu_Panel)
            {
                tempMenuPos = InfoPanel.transform.position.x;
            }

        }
        if (Info_Menu_Panel_Back)
        {
            InfoPanel.transform.position = Vector3.Lerp(InfoPanel.transform.position, InfoOrigPos.transform.position, moveSpeed * Time.deltaTime);

            if (InfoPanel.transform.localPosition.x == tempMenuPos)
            {
                Info_Menu_Panel_Back = false;
                InfoPanel.transform.position = InfoOrigPos.transform.position;
                tempMenuPos = -999999999.99f;
            }
            if (Info_Menu_Panel_Back)
            {
                tempMenuPos = InfoPanel.transform.position.x;
            }
        }
    }

    public void MovePanel()
    {
        Info_Menu_Panel_Back = false;
        Info_Menu_Panel = true;

    }

    public void MovePanelBack()
    {
        Info_Menu_Panel = false;
        Info_Menu_Panel_Back = true;

    }

}
