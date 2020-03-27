using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class AppManager : MonoBehaviour
{
    public List<string> orderItems;
    public bool mActivated = true;
    private string mCurrentAssetName, clear;
    private Text mcountText;
    public int panId;
    public int count, restaurant_id;
    private Button addBtn = null;
    
    public Button g,back;
    private Button remBtn;
    public Sprite[] mButtonMask;
    private Button shwBtn;
    public string mCurrentItemCount = "0";
    //public LoadCategory panIdContainer;
    private static AppManager instanceRef;
    private Restaurants sn;
    public List<ARItems> orders,temp_orders;
    public ARItems mCurrentInstance;
    AndroidJavaObject currentActivity;
    public GameObject loadingpanel;
    public GameObject mBundleInstance = null;
    public bool mAttached = true;
    public AssetBundle bundle;
    private string shareSubject, shareMessage;
    private bool isProcessing = false;
    private string screenshotName;
    private bool isFocus = false;
    int flag_order,flag=0;
    string json,android_order;

    //Make the current object persistant throughtout all the scenes
    void Awake()
    {
        if (instanceRef == null)
        {
            instanceRef = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        //Fetch the firebaseloader object
        Debug.Log("App manager");
         android_order = GameObject.Find("ImageTarget").GetComponent<ARLoader>().android_order;
        Debug.Log("Android order in start of App manager:"+android_order);
        clear = GameObject.Find("ImageTarget").GetComponent<ARLoader>().clear;
        Debug.Log("Clear in start of App manager:" + clear);
        orderItems = new List<string>();
        orders = new List<ARItems>();
        mActivated = true;
        if (!android_order.Equals("nodata"))
        {
           
           var a1 = JsonUtility.FromJson<WrapperClass>(android_order);
            temp_orders = a1.list;
            Debug.Log("Count of temp_orders in start " + temp_orders.Count);
            if (temp_orders.Count < orders.Count || temp_orders.Count > orders.Count)
            {
                orders = temp_orders;
            }
            else
            {
                orders = temp_orders;
            }
            Debug.Log("Count of orders in start " + orders.Count);
           
        }

        flag = 1;



    }
    void OnApplicationFocus(bool focus)
    {
        if (focus && flag==1)
        {
            android_order = GameObject.Find("ImageTarget").GetComponent<ARLoader>().android_order;
            Debug.Log("Android order in focu of App manager:" + android_order);
            clear = GameObject.Find("ImageTarget").GetComponent<ARLoader>().clear;
            Debug.Log("App manager focus:"+clear);

            if (clear.Equals("yes"))
            {

                Debug.Log("in clear");
                orders.Clear();
                orderItems.Clear();
            }
           
            if (!android_order.Equals("nodata"))
            {
                Debug.Log("In nodata");
                var a1 = JsonUtility.FromJson<WrapperClass>(android_order);
                temp_orders = a1.list;
                Debug.Log("Count of temp_orders in focus  " + temp_orders.Count);

                if (temp_orders.Count < orders.Count || temp_orders.Count > orders.Count)
                {
                    orders = temp_orders;
                }
                else
                {
                    //JsonUtility.FromJsonOverwrite(android_order, orders);
                    orders = temp_orders;

                }
                Debug.Log("Count of orders in focus" + orders.Count);

            }
        }
        


    }
    void FixedUpdate()
    {
        //Check if the current scene is AR view 
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("ARMainScreen_3"))
        {
            //Null check on BundleInstance

            clear = GameObject.Find("ImageTarget").GetComponent<ARLoader>().clear;
            android_order = GameObject.Find("ImageTarget").GetComponent<ARLoader>().android_order;
          //  Debug.Log("Android order in fixed update of App manager:" + android_order);


            mBundleInstance = GameObject.Find("ImageTarget").GetComponent<ARLoader>().mBundleInstance;
            mAttached = GameObject.Find("ImageTarget").GetComponent<ARLoader>().mAttached;
            bundle = GameObject.Find("ImageTarget").GetComponent<ARLoader>().bundle;
            string temp = GameObject.Find("ImageTarget").GetComponent<ARLoader>().temp;
            sn = JsonUtility.FromJson<Restaurants>(temp);
           
           
            


            restaurant_id = GameObject.Find("ImageTarget").GetComponent<ARLoader>().restaurant_id;

            panId = GameObject.Find("ImageTarget").GetComponent<ARLoader>().category_id;


            if (Input.GetKey(KeyCode.Escape))
            {
                

                if (bundle != null)
                {
                    // Debug.Log("Testing in previous");
                    bundle.Unload(false);
                }
                //   Debug.Log("Testing Inside focus-13");
                Destroy(mBundleInstance);
                mBundleInstance.SetActive(false);
                //changed on 11/6/19
                mAttached = false;

                AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                if (orders.Count > 0)
                {
                    flag_order = 1;

                    var a = new WrapperClass() { list = orders };
                    json = JsonUtility.ToJson(a);
                }
                else
                {
                    flag_order = 0;
                    json = "null";
                }
                currentActivity.Call("onBackAndroid", flag_order, json, panId);
            }


            
            if (GameObject.Find("ImageTarget").GetComponent<ARLoader>().mBundleInstance != null)
                mCurrentInstance = new ARItems();
            //Create a new ARItems object and assign it to CurrentInstance

            try
            {
                //Set item names & their customization options 
                mCurrentAssetName = GameObject.Find("ImageTarget").GetComponent<ARLoader>().Assename.text;
                mCurrentInstance.name = mCurrentAssetName;
                mCurrentInstance.price = GameObject.Find("ImageTarget").GetComponent<ARLoader>().Price.text.Substring(1);
               // Debug.Log("Price:"+mCurrentInstance.price);
                mCurrentInstance.description = GameObject.Find("ImageTarget").GetComponent<ARLoader>().Description.text;

                // mCurrentInstance.Labels = sn.Restaurants[restaurant_id].Categories[panId].Items[GameObject.Find("ImageTarget").GetComponent<ARLoader>().currentAsset].Customisation;

                //mCurrentInstance.mOptions = new Options[mCurrentInstance.Labels.Length];
                //for (int l = 0; l < mCurrentInstance.Labels.Length; l++)
                //{
                //    mCurrentInstance.mOptions[l] = new Options();
                //    if (mCurrentInstance.Labels[l] =="999")
                //    {
                //        mCurrentInstance.Labels[l] = "null";
                //    }
                //    mCurrentInstance.mOptions[l].label = mCurrentInstance.Labels[l];
                //}

                if (android_order.Equals("nodata") && orders.Count<1)
                {
                    mCurrentInstance.jain = 0;
                    mCurrentInstance.quantity =0;
                    mCurrentInstance.instructions = " ";
                    mCurrentInstance.hasCustomisation = false;
                }
                else
                {
                    if (!orders.Contains(mCurrentInstance))
                    {
                        mCurrentInstance.jain = 0;
                        mCurrentInstance.quantity = 0;
                        mCurrentInstance.instructions = " ";
                        mCurrentInstance.hasCustomisation = false;
                    }
                    else
                    {
                       // Debug.Log("sdasddad");
                        mCurrentInstance.jain = orders[orders.IndexOf(mCurrentInstance)].jain;
                        mCurrentInstance.quantity = orders[orders.IndexOf(mCurrentInstance)].quantity;
                        mCurrentInstance.instructions = orders[orders.IndexOf(mCurrentInstance)].instructions;
                        mCurrentInstance.hasCustomisation=orders[orders.IndexOf(mCurrentInstance)].hasCustomisation;
                        //Debug.Log("Instruction: " + ""+orders[orders.IndexOf(mCurrentInstance)].instructions);

                    }
                    
                }
                
                
                mCurrentAssetName = GameObject.Find("ImageTarget").GetComponent<ARLoader>().Assename.text;
                mCurrentItemCount = "" + mCurrentInstance.quantity;
            }
            catch
            {

            }
            if (addBtn == null || remBtn == null)
            {
                    
                shwBtn = GameObject.Find("ShowTable").GetComponent<Button>();
                remBtn = GameObject.Find("RemoveItem").GetComponent<Button>();
                addBtn = GameObject.Find("AddItem").GetComponent<Button>();
               
                loadingpanel = GameObject.Find("LoadingPanel");
               g = GameObject.Find("ViewOrders").GetComponent<Button>();
                back = GameObject.Find("Back").GetComponent<Button>();
                back.onClick.AddListener(loadPrevious);
               g.GetComponent<Button>().onClick.AddListener(loadScne);
               

                addBtn.onClick.AddListener(AddItem);
               remBtn.onClick.AddListener(RemoveItem);

               mcountText = shwBtn.GetComponentInChildren<Text>();
                mcountText.text = mCurrentItemCount;
                shwBtn.GetComponent<Image>().sprite = mButtonMask[0];
                addBtn.GetComponent<Image>().sprite = mButtonMask[1];
                remBtn.GetComponent<Image>().sprite = mButtonMask[2];


            }
            mcountText.text = mCurrentItemCount;

            //  mCurrentItemCount = "" + orderItems.Count(x => x.Equals(mCurrentAssetName));
            //int index = orders.FindIndex(x => x.Equals(mCurrentAssetName));




            if (orders.Count < 1 && mActivated)
            {
                mActivated = false;
                if (g.gameObject.activeSelf)
                    g.gameObject.SetActive(false);

            }
            //Else if order is not empty, show the View Orders button
            if (orders.Count > 0 && !mActivated)
            {
                mActivated = true;
                if (g != null)
                    g.gameObject.SetActive(true);



            }

        }
        
       
    }

    void loadPrevious()
    {

        if (bundle != null)
        {
            // Debug.Log("Testing in previous");
            bundle.Unload(false);
        }
        //   Debug.Log("Testing Inside focus-13");
        Destroy(mBundleInstance);
        mBundleInstance.SetActive(false);
        //changed on 11/6/19
        mAttached = false;

        AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        if (orders.Count > 0)
        {
           flag_order = 1;

            var a = new WrapperClass() { list = orders };
             json = JsonUtility.ToJson(a);
        }
        else
        {
            flag_order = 0;
            json = "null";
        }
        currentActivity.Call("onBackAndroid",flag_order,json,panId);
    }
    void loadScne()
    {
       
        if (bundle != null)
        {
            bundle.Unload(false);
        }

        Destroy(mBundleInstance);
        mAttached = true;
       
        AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        


        var a = new WrapperClass() { list = orders };
        string json = JsonUtility.ToJson(a);
        
        Debug.Log("JSON" + json);
        loadingpanel.SetActive(true);
        currentActivity.Call("onGameFinish", json);

    }
    void AddItem()
    {
        if (GameObject.Find("ImageTarget").GetComponent<ARLoader>().hasOrdering==1)
        {
            Debug.Log("Show");
            // ShowToast();
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, "Visit the Restaurant and scan the QR on the table to order. Thank you!", 0);
                    toastObject.Call("show");
                }));
            }
           // g.gameObject.SetActive(false);
        }
       else if (mCurrentInstance.hasCustomisation)
        {
            Debug.Log("Show");
           // ShowToast();
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, "This item contains customisation. Please go back to the previous screen to add customisations", 0);
                    toastObject.Call("show");
                }));
            }
            //g.gameObject.SetActive(false);
        }
        else
        {
            int q = mCurrentInstance.quantity + 1;
            Debug.Log("Quantity: " + q);
            mCurrentItemCount = "" + q;
            mcountText.text = mCurrentItemCount;
            //orderItems.Add(mCurrentAssetName);
            //mCurrentItemCount = "" + orderItems.Count(x => x.Equals(mCurrentAssetName));
            if (mCurrentItemCount != "0")
            {
               // GameObject.Find("Popup").transform.localScale = new Vector2(1f, 1f);
               // StartCoroutine(wait());
                if (!orders.Contains(mCurrentInstance))
                {
                    //mCurrentInstance.quantity = orderItems.Count(x => x.Equals(mCurrentAssetName));
                    Debug.Log("Inside item if");
                    mCurrentInstance.quantity = q;
                    // mCurrentInstance.instructions = orders[orders.IndexOf(mCurrentInstance)].instructions;
                    orders.Add(mCurrentInstance);
                    //  Debug.Log(mCurrentInstance.name + "  " + mCurrentInstance.price + " Added to the Order List" + orders.Count);
                }
                else
                {
                    Debug.Log("Inside item else");
                    // orders[orders.IndexOf(mCurrentInstance)].quantity = orderItems.Count(x => x.Equals(mCurrentAssetName));
                    orders[orders.IndexOf(mCurrentInstance)].quantity = q;
                    orders[orders.IndexOf(mCurrentInstance)].instructions = mCurrentInstance.instructions;
                }
            }
      


        }



    }
    void ShowToast()
    {
         GameObject.Find("Popup").transform.localScale = new Vector2(1f, 1f);
            StartCoroutine(wait());
         
          
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(1f);
        try
        {
            GameObject.Find("Popup").transform.localScale = new Vector2(1f, 0f);
        }
        catch
        {
            Debug.Log("Popup Missed");
        }
    }
    void RemoveItem()
    {
        if (GameObject.Find("ImageTarget").GetComponent<ARLoader>().hasOrdering==1)
        {
            Debug.Log("Show");
            // ShowToast();
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, "Visit the Restaurant and scan the QR on the table to order. Thank you!", 0);
                    toastObject.Call("show");
                }));
            }
        }
        if (mCurrentInstance.hasCustomisation)
        {
           // Debug.Log("Show");
            // ShowToast();
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, "This item contains customisation. Please go back to the previous screen to add customisations", 0);
                    toastObject.Call("show");
                }));
            }
        }
        else
        {
            int q = mCurrentInstance.quantity - 1;
            if (orderItems.Contains(mCurrentAssetName))
            {
                orderItems.Remove(mCurrentAssetName);


            }
            if (orders.Contains(mCurrentInstance))
            {
                if (q == 0)
                {
                    orders.Remove(mCurrentInstance);
                }
                else
                {
                    orders[orders.IndexOf(mCurrentInstance)].quantity = q;
                    orders[orders.IndexOf(mCurrentInstance)].instructions = mCurrentInstance.instructions;
                }
              
                

            }
        }     
    }

    public void Share()
    {

        StartCoroutine(TakeSSAndShare());
    }


    private IEnumerator TakeSSAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        new NativeShare().AddFile(filePath).SetSubject("Food").SetText("Checkout the "+ mCurrentAssetName+" at "+ sn.Name).Share();

        
    }

   

}



[Serializable]
public class ARItems : IEquatable<ARItems>
{

    public string name = "";
    public string price = "0";
    public bool hasCustomisation;
    public int jain = 0;
    public int quantity = 0;
    public string customisation = "";
    public int optionprice = 0;
    public string instructions = "";
    public string description = "";
    public string category;

    //public string[] Labels;
    //public Options[] mOptions;
    public ARItems(string name = "", string price = "", int jain = 0, int quantity = 0, string instructions = "",string description="",string customisation="",int optionprice=0,bool has=false,string category="")
    {
        this.name = name;
        this.price = price;
        this.jain = jain;
        this.quantity = quantity;
        this.instructions = instructions;
        this.description = description;
        this.customisation = customisation;
        this.optionprice = optionprice;
        this.hasCustomisation = has;
        this.category = category;
    }

    public bool Equals(ARItems aritem)
    {

        return this.name == aritem.name &&
        this.price == aritem.price;


    }
    public override int GetHashCode()
    {
        return this.name.GetHashCode();
    }



}
//[Serializable]
//public class Options
//{
//    public string label;
//    public bool isOn;

//}

[Serializable]
public class WrapperClass
{
    public List<ARItems> list;
   // public ARItems[] list;
}

