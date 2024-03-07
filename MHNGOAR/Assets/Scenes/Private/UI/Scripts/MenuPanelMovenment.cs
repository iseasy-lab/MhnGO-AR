using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelMovenment : MonoBehaviour
{
    public GameObject MenuOrigPos;
    public GameObject MenuActivePos;
    public GameObject MenuPanel;

    public bool Move_Menu_Panel, Move_Menu_Panel_Back;

    public float moveSpeed;
    public float tempMenuPos;

    // Start is called before the first frame update
    void Start()
    {
        MenuPanel.transform.position = MenuOrigPos.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (Move_Menu_Panel)
        {
            MenuPanel.transform.position = Vector3.Lerp(MenuPanel.transform.position, MenuActivePos.transform.position, moveSpeed * Time.deltaTime);

            if (MenuPanel.transform.localPosition.x == tempMenuPos)
            {
                Move_Menu_Panel = false;
                MenuPanel.transform.position = MenuActivePos.transform.position;
                tempMenuPos = -999999999.99f;
            }
            if (Move_Menu_Panel)
            {
                tempMenuPos = MenuPanel.transform.position.x;
            }

        }
        if (Move_Menu_Panel_Back)
        {
            MenuPanel.transform.position = Vector3.Lerp(MenuPanel.transform.position, MenuOrigPos.transform.position, moveSpeed * Time.deltaTime);

            if (MenuPanel.transform.localPosition.x == tempMenuPos)
            {
                Move_Menu_Panel_Back = false;
                MenuPanel.transform.position = MenuOrigPos.transform.position;
                tempMenuPos = -999999999.99f;
            }
            if (Move_Menu_Panel_Back)
            {
                tempMenuPos = MenuPanel.transform.position.x;
            }
        }
    }

    public void MovePanel()
    {
        Move_Menu_Panel_Back = false;
        Move_Menu_Panel = true;

    }

    public void MovePanelBack()
    {
        Move_Menu_Panel = false;
        Move_Menu_Panel_Back = true;

    }

}
