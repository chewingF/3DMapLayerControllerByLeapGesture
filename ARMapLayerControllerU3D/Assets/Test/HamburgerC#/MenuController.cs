using Mapbox.Examples;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuController {
    //to control all UI of the menu including layer properties, new layers.

    protected Transform user;

    //initial height for all menu items
    protected float initialHeight = 1;
    protected int initialFontSize = 80;
    protected int HighLightFontSize = 120;

    //all menu states
    protected bool rightMenuOn = false;
    protected bool leftMenuOn = false;
    protected bool rightMenuSubOn = false;

    //rightmenu specific variables
    protected string rightMenuTitle = "Right";
    protected string rightMenuTitleName = "RightMenuTitle";
    protected List<string> rightMenuItems = new List<string>();
    protected float rightMenuDegree = 30;
    protected float rightMenuGap = .3f;
    protected int rightMenuSelection = -1;
    protected string rightMenuItemName = "RightMenuItem";

    //leftmenu specific variables
    protected string leftMenuTitle = "Left";
    protected string leftMenuTitleName = "LeftMenuTitle";
    protected List<string> leftMenuItems = new List<string>();
    protected float leftMenuDegree = -30;
    protected float leftMenuGap = .3f;
    protected int leftMenuSelection = -1;
    protected string leftMenuItemName = "LeftMenuItem";

    //rightmenuSub specific variables
    protected string rightMenuSubTitle = "RightSub";
    protected string rightMenuSubTitleName = "RightMenuSubTitle";
    protected List<string> rightMenuSubItems = new List<string>();
    protected float rightMenuSubDegree = 60;
    protected float rightMenuSubGap = .3f;
    protected int rightMenuSubSelection = -1;
    protected string rightMenuSubItemName = "RightMenuSubItem";



    public MenuController()
    {
    }

    public float getRightMenuDegree()
    {
        return rightMenuDegree;
    }

    public float getLeftMenuDegree()
    {
        return leftMenuDegree;
    }

    public float getRightMenuSubDegree()
    {
        return rightMenuSubDegree;
    }

    public string getRightMenuCMD()
    {
        if (rightMenuItems.Count == 0) {
            return null;
        }
        return rightMenuItems[rightMenuSelection];
    }

    public string getLeftMenuCMD()
    {
        if (leftMenuItems.Count == 0) {
            return null;
        }
        return leftMenuItems[leftMenuSelection];
    }

    public string getRightMenuSubCMD()
    {
        if (rightMenuSubItems.Count == 0) {
            return null;
        }
        return rightMenuSubItems[rightMenuSubSelection];
    }

    public bool isRightMenuOn()
    {
        return rightMenuOn;
    }

    public bool isLefttMenuOn()
    {
        return leftMenuOn;
    }

    public bool isRightMenuSubOn()
    {
        return rightMenuSubOn;
    }

    //Create Menu Methods
    public void RightMenuCreate(string rightMenuTitle, List<string> rightMenuItems, Transform target, float degree, float gap)
    {
        if (rightMenuOn)
        {
            return;
        }
        this.rightMenuTitle = rightMenuTitle;
        this.rightMenuItems = rightMenuItems;
        user = target;
        rightMenuDegree = degree;
        rightMenuGap = gap;
        rightMenuSelection = 0;
        GameObject theText;
        TextMesh textMesh;
        /*
        //set title 
        var theText = new GameObject(rightMenuTitleName);
        theText.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
        //theText.AddComponent<CameraBillboard>();
        var textMesh = theText.AddComponent<TextMesh>();
        textMesh.text = this.rightMenuTitle;
        textMesh.fontSize = initialFontSize;
        theText.transform.RotateAround(target.transform.position, Vector3.up, rightMenuDegree);
        theText.transform.Translate(Vector3.up * (initialHeight + (4 * rightMenuGap)));
        */
        //set items
        int count = 0;
        foreach (string str in this.rightMenuItems)
        {
            theText = new GameObject(rightMenuItemName + count);
            theText.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
            //theText.AddComponent<CameraBillboard>();
            textMesh = theText.AddComponent<TextMesh>();
            textMesh.text = str;
            textMesh.fontSize = initialFontSize;
            theText.transform.RotateAround(target.transform.position, Vector3.up, rightMenuDegree);
            theText.transform.Translate(Vector3.up * (initialHeight - ((count + 1) * rightMenuGap)));
            count += 1;
        }
        rightMenuOn = true;
        RightMenuHighLight();
    }

    public void LeftMenuCreate(string leftMenuTitle, List<string> leftMenuItems, Transform target, float degree, float gap)
    {
        if (leftMenuOn)
        {
            return;
        }
        this.leftMenuItems = leftMenuItems;
        this.leftMenuTitle = leftMenuTitle;
        user = target;
        leftMenuDegree = degree;
        leftMenuGap = gap;
        leftMenuSelection = 0;
        GameObject theText;
        TextMesh textMesh;
        /*
        //set title 
        var theText = new GameObject(leftMenuTitleName);
        theText.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
        //theText.AddComponent<CameraBillboard>();
        var textMesh = theText.AddComponent<TextMesh>();
        textMesh.text = this.leftMenuTitle;
        textMesh.fontSize = initialFontSize;
        theText.transform.RotateAround(target.transform.position, Vector3.up, leftMenuDegree);
        theText.transform.Translate(Vector3.up * (initialHeight + (4 * leftMenuGap)));
        */
        //set items
        int count = 0;
        foreach (string str in this.leftMenuItems)
        {
            theText = new GameObject(leftMenuItemName + count);
            theText.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
            //theText.AddComponent<CameraBillboard>();
            textMesh = theText.AddComponent<TextMesh>();
            textMesh.text = str;
            textMesh.fontSize = initialFontSize;
            theText.transform.RotateAround(target.transform.position, Vector3.up, leftMenuDegree);
            theText.transform.Translate(Vector3.up * (initialHeight - ((count + 1) * leftMenuGap)));
            count += 1;
        }
        leftMenuOn = true;
        LeftMenuHighLight();
    }

    public void RightMenuSubCreate(string rightMenuSubTitle, List<string> rightMenuSubItems, Transform target, float degree, float gap)
    {
        if (rightMenuSubOn || !rightMenuOn)
        {
            return;
        }
        this.rightMenuSubItems = rightMenuSubItems;
        this.rightMenuSubTitle = rightMenuSubTitle;
        user = target;
        rightMenuSubDegree = rightMenuDegree + degree;
        rightMenuSubGap = gap;
        rightMenuSubSelection = 0;
        GameObject theText;
        TextMesh textMesh;
        /*
        //set title 
        var theText = new GameObject(rightMenuSubTitleName);
        theText.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
        //theText.AddComponent<CameraBillboard>();
        var textMesh = theText.AddComponent<TextMesh>();
        textMesh.text = this.rightMenuSubTitle;
        textMesh.fontSize = initialFontSize;
        theText.transform.RotateAround(target.transform.position, Vector3.up, rightMenuSubDegree);
        theText.transform.Translate(Vector3.up * (initialHeight + (4 * rightMenuSubGap)));
        */
        //set items
        int count = 0;
        foreach (string str in this.rightMenuSubItems)
        {
            theText = new GameObject(rightMenuSubItemName + count);
            theText.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
            //theText.AddComponent<CameraBillboard>();
            textMesh = theText.AddComponent<TextMesh>();
            textMesh.text = str;
            textMesh.fontSize = initialFontSize;
            theText.transform.RotateAround(target.transform.position, Vector3.up, rightMenuSubDegree);
            theText.transform.Translate(Vector3.up * (initialHeight - ((count + 1) * rightMenuSubGap)));
            count += 1;
        }
        rightMenuSubOn = true;
        RightMenuSubHighLight();
    }


    //Refresh Menu Methods
    public void RightMenuRefresh(List<string> rightMenuItems)
    {
        if (!rightMenuOn)
        {
            return;
        }
        else
        {
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(rightMenuItemName)))
            {
                GameObject.Destroy(go);
            }
            this.rightMenuItems = rightMenuItems;
            if (rightMenuSelection > rightMenuItems.Count - 1)
            {
                rightMenuSelection = rightMenuItems.Count - 1;
            }
            int count = 0;
            foreach (string str in this.rightMenuItems)
            {
                var theText = new GameObject(rightMenuItemName + count);
                theText.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
                //theText.AddComponent<CameraBillboard>();
                var textMesh = theText.AddComponent<TextMesh>();
                textMesh.text = str;
                textMesh.fontSize = initialFontSize;
                theText.transform.RotateAround(user.transform.position, Vector3.up, rightMenuDegree);
                theText.transform.Translate(Vector3.up * (initialHeight - ((count + 1 - rightMenuSelection) * rightMenuGap)));
                count += 1;
            }
        }
        RightMenuHighLight();
    }
    public void RightMenuSubRefresh(List<string> rightMenuSubItems)
    {
        if (!rightMenuSubOn)
        {
            return;
        }
        else
        {
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(rightMenuSubItemName)))
            {
                GameObject.Destroy(go);
            }
            this.rightMenuSubItems = rightMenuSubItems;
            if (rightMenuSubSelection > rightMenuSubItems.Count - 1)
            {
                rightMenuSubSelection = rightMenuSubItems.Count - 1;
            }
            int count = 0;
            foreach (string str in this.rightMenuSubItems)
            {
                var theText = new GameObject(rightMenuSubItemName + count);
                theText.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
                //theText.AddComponent<CameraBillboard>();
                var textMesh = theText.AddComponent<TextMesh>();
                textMesh.text = str;
                textMesh.fontSize = initialFontSize;
                theText.transform.RotateAround(user.transform.position, Vector3.up, rightMenuSubDegree);
                theText.transform.Translate(Vector3.up * (initialHeight - ((count + 1 - rightMenuSubSelection) * rightMenuSubGap)));
                count += 1;
            }
        }
        RightMenuSubHighLight();
    }

    public void LeftMenuRefresh(List<string> leftMenuItems)
    {
        if (!leftMenuOn)
        {
            return;
        }
        else
        {
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(leftMenuItemName)))
            {
                GameObject.Destroy(go);
            }
            this.leftMenuItems = leftMenuItems;
            if (leftMenuSelection > leftMenuItems.Count - 1)
            {
                leftMenuSelection = leftMenuItems.Count - 1;
            }
            if (leftMenuSelection < 0)
            {
                leftMenuSelection = 0;
            }
            int count = 0;
            foreach (string str in this.leftMenuItems)
            {
                var theText = new GameObject(leftMenuItemName + count);
                theText.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
                //theText.AddComponent<CameraBillboard>();
                var textMesh = theText.AddComponent<TextMesh>();
                textMesh.text = str;
                textMesh.fontSize = initialFontSize;
                theText.transform.RotateAround(user.transform.position, Vector3.up, leftMenuDegree);
                theText.transform.Translate(Vector3.up * (initialHeight - ((count + 1 - leftMenuSelection) * leftMenuGap)));
                count += 1;
            }
        }
        LeftMenuHighLight();
    }
    
    //Close Menu Methods
    public void RightMenuClose()
    {
        GameObject.Destroy(GameObject.Find(rightMenuTitleName));
        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(rightMenuItemName)))
        {
            GameObject.Destroy(go);
        }
        rightMenuItems = new List<string>();
        rightMenuGap = 0;
        rightMenuSelection = -1;
        rightMenuOn = false;
    }

    public void LeftMenuClose()
    {
        GameObject.Destroy(GameObject.Find(leftMenuTitleName));
        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(leftMenuItemName)))
        {
            GameObject.Destroy(go);
        }
        leftMenuItems = new List<string>();
        leftMenuGap = 0;
        leftMenuSelection = -1;
        leftMenuOn = false;
    }

    public void RightMenuSubClose()
    {
        GameObject.Destroy(GameObject.Find(rightMenuSubTitleName));
        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(rightMenuSubItemName)))
        {
            GameObject.Destroy(go);
        }
        rightMenuSubItems = new List<string>();
        rightMenuSubGap = 0;
        rightMenuSubSelection = -1;
        rightMenuSubOn = false;
    }


    //Select Menu Item Methods
    public void RightMenuSelectLast()
    {
        if (!rightMenuOn)
        {
            return;
        }
        else if (rightMenuSelection >= (rightMenuItems.Count - 1))
        {
            return;
        }
        else
        {
            rightMenuSelection += 1;
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(rightMenuItemName)))
            {
                go.transform.Translate(Vector3.up * rightMenuGap);
            }
        }
        RightMenuHighLight();
    }

    public void LeftMenuSelectLast()
    {
        if (!leftMenuOn)
        {
            return;
        }
        else if (leftMenuSelection >= (leftMenuItems.Count - 1))
        {
            return;
        }
        else
        {
            leftMenuSelection += 1;
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(leftMenuItemName)))
            {
                go.transform.Translate(Vector3.up * leftMenuGap);
            }
        }
        LeftMenuHighLight();
    }

    public void RightMenuSubSelectLast()
    {
        if (!rightMenuSubOn)
        {
            return;
        }
        else if (rightMenuSubSelection >= (rightMenuSubItems.Count - 1))
        {
            return;
        }
        else
        {
            rightMenuSubSelection += 1;
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(rightMenuSubItemName)))
            {
                go.transform.Translate(Vector3.up * rightMenuSubGap);
            }
        }
        RightMenuSubHighLight();
    }

    public void RightMenuSelectNext()
    {
        if (!rightMenuOn)
        {
            return;
        }
        else if (rightMenuSelection <= 0)
        {
            return;
        }
        else
        {
            rightMenuSelection -= 1;
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(rightMenuItemName)))
            {
                go.transform.Translate(Vector3.down * rightMenuGap);
            }
        }
        RightMenuHighLight();
    }

    public void LeftMenuSelectNext()
    {
        if (!leftMenuOn)
        {
            return;
        }
        else if (leftMenuSelection <= 0)
        {
            return;
        }
        else
        {
            leftMenuSelection -= 1;
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(leftMenuItemName)))
            {
                go.transform.Translate(Vector3.down * leftMenuGap);
            }
        }
        LeftMenuHighLight();
    }

    public void RightMenuSubSelectNext()
    {
        if (!rightMenuSubOn)
        {
            return;
        }
        else if (rightMenuSubSelection <= 0)
        {
            return;
        }
        else
        {
            rightMenuSubSelection -= 1;
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(rightMenuSubItemName)))
            {
                go.transform.Translate(Vector3.down * rightMenuSubGap);
            }
        }
        RightMenuSubHighLight();
    }

    //Change title methods
    public void RightMenuChangeTitle(string rightMenuTitle)
    {
        if (!rightMenuOn)
        {
            return;
        }
        this.rightMenuTitle = rightMenuTitle;
        GameObject theText = GameObject.Find(rightMenuTitleName);
        theText.GetComponent<TextMesh>().text = this.rightMenuTitle;
    }

    public void LeftMenuChangeTitle(string leftMenuTitle)
    {
        if (!leftMenuOn)
        {
            return;
        }
        this.leftMenuTitle = leftMenuTitle;
        GameObject theText = GameObject.Find(leftMenuTitleName);
        theText.GetComponent<TextMesh>().text = this.leftMenuTitle;
    }

    public void RightMenuSubChangeTitle(string rightMenuSubTitle)
    {
        if (!rightMenuSubOn)
        {
            return;
        }
        this.rightMenuSubTitle = rightMenuSubTitle;
        GameObject theText = GameObject.Find(rightMenuSubTitleName);
        theText.GetComponent<TextMesh>().text = this.rightMenuSubTitle;
    }

    //Highlight menu title Methods
    public void WatchOn(int i)
    {
        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(rightMenuItemName) || obj.name.Contains(leftMenuItemName) || obj.name.Contains(rightMenuSubItemName)))
        {
            var textMesh = go.GetComponent<TextMesh>();
            textMesh.color = Color.gray;
        }
        switch(i){
            case 0:
                LeftMenuHighLight();
                return;
            case 1:
                RightMenuHighLight();
                return;
            case 2:
                RightMenuSubHighLight();
                return;
        }

    }

    //HighLight Selection Methods
    protected void RightMenuHighLight()
    {
        if (!rightMenuOn)
        {
            return;
        }
        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(rightMenuItemName)))
        {
            var textMesh = go.GetComponent<TextMesh>();
            if (go.name != (rightMenuItemName + rightMenuSelection))
            {
                textMesh.fontSize = initialFontSize;
                textMesh.color = Color.white;
            }
            else
            {
                textMesh.fontSize = HighLightFontSize;
                textMesh.color = Color.black;
            }
        }
    }

    protected void LeftMenuHighLight()
    {
        if (!leftMenuOn)
        {
            return;
        }
        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(leftMenuItemName)))
        {
            var textMesh = go.GetComponent<TextMesh>();
            if (go.name != (leftMenuItemName + leftMenuSelection))
            {
                textMesh.fontSize = initialFontSize;
                textMesh.color = Color.white;
            }
            else
            {
                textMesh.fontSize = HighLightFontSize;
                textMesh.color = Color.black;
            }
        }
    }

    protected void RightMenuSubHighLight()
    {
        if (!rightMenuSubOn)
        {
            return;
        }
        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(rightMenuSubItemName)))
        {
            var textMesh = go.GetComponent<TextMesh>();
            if (go.name != (rightMenuSubItemName + rightMenuSubSelection))
            {
                textMesh.fontSize = initialFontSize;
                textMesh.color = Color.white;
            }
            else
            {
                textMesh.fontSize = HighLightFontSize;
                textMesh.color = Color.black;
            }
        }
    }
}
