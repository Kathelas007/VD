﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PieChart.ViitorCloud
{
    public class ClickEffect : MonoBehaviour
    {
        private TextMeshProUGUI age;
        private TextMeshProUGUI gender;
        private TextMeshProUGUI licence;
        private TextMeshProUGUI navType;
        public static ClickEffect isn { set; get; }

        internal Dictionary<GameObject, GameObject> allParsObjectAndCanvasObjectDict = new Dictionary<GameObject, GameObject>();

        GameObject targetedObject;
        internal Camera Maincamera;
        RaycastHit hit;
        private Vector3 position;
        GameObject tempObject;

        // Start is called before the first frame update
        void Awake()
        {
            isn = this;
            Maincamera = GetComponent<Camera>();
            targetedObject = gameObject;

            GameObject ui = GameObject.Find("VarInfo");
            age = ui.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            gender = ui.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            licence = ui.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            navType = ui.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Maincamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log(hit.collider.transform.name);

                    if (hit.collider.gameObject.CompareTag("Part"))
                    {
                        //Debug.Log(hit.collider.transform.tag);
                        //Debug.Log(hit.collider.transform.name);
                        StaticFiltrationController.ChangeFiltration(hit.collider.transform.name);

                        age.text = StaticFiltrationController.age.ToString();
                        licence.text = StaticFiltrationController.licence.ToString();
                        navType.text = StaticFiltrationController.navType.ToString();
                        gender.text = StaticFiltrationController.gender.ToString();

                        if (targetedObject != hit.collider.gameObject)
                            hit.collider.gameObject.GetComponent<PartProperties>().MyCurrntPosition = hit.collider.transform.position;
                    
                        if (targetedObject != Maincamera.gameObject && hit.collider != null)
                        {

                           
                            if (targetedObject != hit.collider.gameObject)
                            {
                                targetedObject.transform.position = targetedObject.GetComponent<PartProperties>().MyCurrntPosition;
                                targetedObject = hit.collider.gameObject;
                            }
                        }
                        if (targetedObject == Maincamera.gameObject)
                            targetedObject = hit.collider.gameObject;
                    }
                }
                else
                {
                    foreach (KeyValuePair<GameObject, GameObject> item in allParsObjectAndCanvasObjectDict)
                    {
                        if (item.Value.GetComponent<Image>().color.a < 0.2)
                        {
                            ReverseAnimation(item.Value.GetComponent<Animation>(), "reducethealpha");
                            //Debug.Log("[recp");
                        }
                    }
                    if (targetedObject != null)
                    {
                        if (targetedObject.GetComponent<PartProperties>() != null)
                            targetedObject.transform.position = targetedObject.GetComponent<PartProperties>().MyCurrntPosition;
                        targetedObject = Maincamera.gameObject;
                    }
                }
            }
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Part"))
                {
                    float dis = Vector3.Distance(hit.transform.position, hit.transform.GetComponent<PartProperties>().forward);
                    if (dis > 1.1f)
                    {
                        hit.transform.position = Vector3.Lerp(hit.transform.position, hit.transform.GetComponent<PartProperties>().forward, 1 * Time.deltaTime);
                    }
                }
            }   
                //Raycast();
            }


        void Raycast()
        {
            Ray ray = Maincamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit0))
            {
                if (tempObject != null)
                    if (tempObject != hit0.collider.gameObject)
                    {
                        if (tempObject.GetComponent<cakeslice.Outline>() != null)
                        {
                            tempObject.GetComponent<cakeslice.Outline>().enabled = false;
                            tempObject.GetComponent<PartProperties>().SetAnimation(false);
                        }
                    }

                tempObject = hit0.collider.gameObject;
                tempObject.GetComponent<cakeslice.Outline>().enabled = true;
                tempObject.GetComponent<PartProperties>().SetAnimation(true);
            }
            else
            {
                if (tempObject != null)
                {
                    if (tempObject.GetComponent<cakeslice.Outline>() != null)
                    {
                        tempObject.GetComponent<cakeslice.Outline>().enabled = false;
                        tempObject.GetComponent<PartProperties>().SetAnimation(false);
                    }
                }
            }
        }
        public void ReverseAnimation(Animation anim, string AnimationName)
        {
            anim[AnimationName].speed = -1;
            anim[AnimationName].time = anim[AnimationName].length;
            anim.CrossFade(AnimationName);
        }
    }
}