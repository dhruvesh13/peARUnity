using UnityEngine;
using UnityEditor;
using System.IO;
using Vuforia;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class ModelLoader : MonoBehaviour
{
    private Sprite showExpanded;
    [Header("All Other Crap XD")]
    public Sprite hideExpanded;
    public Text progressBar;
    public Button[] navigation;
    public RawImage pan;
    public AssetBundle bundle;
    public Text Assename;
    public AssetBundle assetBun;
    public string BundleURL;
    private WWW www;
    public string AssetName;
    public Restaurants sn;
    public int version;
    public GameObject parent;

    // public AppManager app;
    public int pesen;
    private int bundleSize;
    public int currentAsset, restaurant_id, category_id;
    public GameObject mBundleInstance = null;
    private TrackableBehaviour mTrackableBehaviour;
    public bool mAttached = false, isPaused = false;
    public string[] ItemNames;
    public Text Price;
    public Text Description;
    private GameObject mContent;
    public GameObject mContentPrefab;
    private AssetBundle mBundle;
    private GameObject mExpandedView;
    private GameObject[] mButtonInstance;
    private Button mExpand;
    private Button mAddItem;
    public Dropdown drop;
    public GameObject loadingpanel;
    public string temp, android_order;
    AndroidJavaObject currentActivity;
    public int flag = 0, safety = 0;
    public Button back;
    public string clear = "null";
    AndroidJavaClass UnityPlayer;
    public string assetName;
    private bool focus = false;
    public int hasOrdering = 0;
    List<string> dropDownoptions;
    public List<Items> items;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("mAttached during start: " + mAttached);
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        mExpandedView = GameObject.Find("ExpandedView");
        mContent = GameObject.Find("ListContent");

        mExpand = GameObject.Find("Expand").GetComponent<Button>();
        mExpand.onClick.AddListener(ActivateExpandedView);
        loadingpanel = GameObject.Find("LoadingPanel");
        back = GameObject.Find("Back").GetComponent<Button>();
        // back.onClick.AddListener(loadPrevious);
        //drop = GetComponent<Dropdown>();
        showExpanded = mExpand.GetComponent<UnityEngine.UI.Image>().sprite;

       
        UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
        bool hasExtra = intent.Call<bool>("hasExtra", "Data");

        if (hasExtra)
        {
            //Get intent from categoryList
            AndroidJavaObject extras = intent.Call<AndroidJavaObject>("getExtras");
            temp = extras.Call<String>("getString", "Data");
            android_order = extras.Call<String>("getString", "Order_Data");
            restaurant_id = extras.Call<int>("getInt", "Restaurant");
            category_id = extras.Call<int>("getInt", "Category");
            currentAsset = extras.Call<int>("getInt", "CurrentAsset");
            clear = extras.Call<String>("getString", "Clear");
            hasOrdering = extras.Call<int>("getInt", "hasOrdering");
        }

        //  currentActivity.Call("Analytics");
        sn = JsonUtility.FromJson<Restaurants>(temp);
        items = new List<Items>();
        for (int i = 0; i < sn.Categories[category_id].Items.Length; i++)
        {
            if (sn.Categories[category_id].Items[i].AR == 1)
            {
                items.Add(sn.Categories[category_id].Items[i]);
            }
        }
        Debug.Log("Items size:" + temp);
        Debug.Log("Order:" + android_order);
        Debug.Log("Clear:" + clear);
        Debug.Log("Category_id in start:" + category_id);
        Debug.Log("hasOrdering in start:" + hasOrdering);
        dropDownoptions = new List<string>();

        for (int j = 0; j < sn.Categories.Length; j++)
        {
            int f = 0;
            //Debug.Log("Category:" + sn.Restaurants[restaurant_id].Categories[i].Category_name);

            for (int i = 0; i < sn.Categories[j].Items.Length; i++)
            {
                if (sn.Categories[j].Items[i].AR == 1)
                {
                    f = 1;
                    break;
                }
            }
            if (f == 1)
            {
                dropDownoptions.Add(sn.Categories[j].Category_name);
            }

        }
        drop.ClearOptions();
        drop.AddOptions(dropDownoptions);
        drop.RefreshShownValue();

        navigation[0].onClick.AddListener(LoadPrev);
        navigation[1].onClick.AddListener(LoadNext);



        //AssetBundle Fetching & Caching
        StartCoroutine(DownloadAndCache());

      

        flag = 1;

    }
    void OnApplicationFocus(bool focus)
    {

        if (!focus)
        {
            Debug.Log("mAttached in not focus: " + mAttached);
            Debug.Log("Focus lost");
            mAttached = false;
            Debug.Log("mAttached in not focus after: " + mAttached);
            if (bundle != null)
            {
                bundle.Unload(false);
            }

            //for (int i = 0; i < bundleSize; i++)
            //{
            //    Destroy(mButtonInstance[i]);
            //}
            if (mBundleInstance)
            {
                Destroy(mBundleInstance);
            }
        }

        if (focus && flag == 1)
        {
            // Destroy(mContent);
            Debug.Log("mAttached in focus: " + mAttached);
            Debug.Log("Focus");
            for (int i = 0; i < bundleSize; i++)
            {
                Destroy(mButtonInstance[i]);
            }

            Screen.orientation = ScreenOrientation.LandscapeLeft;

            //drop = GetComponent<Dropdown>();
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            // currentActivity.Call("Analytics");

            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            bool hasExtra = intent.Call<bool>("hasExtra", "Data");

            if (hasExtra)
            {
                AndroidJavaObject extras = intent.Call<AndroidJavaObject>("getExtras");
                temp = extras.Call<String>("getString", "Data");
                android_order = extras.Call<String>("getString", "Order_Data");
                restaurant_id = extras.Call<int>("getInt", "Restaurant");
                category_id = extras.Call<int>("getInt", "Category");
                currentAsset = extras.Call<int>("getInt", "CurrentAsset");
                clear = extras.Call<String>("getString", "Clear");
                hasOrdering = extras.Call<int>("getInt", "hasOrdering");
            }

            sn = JsonUtility.FromJson<Restaurants>(temp);
            items = new List<Items>();
            for (int i = 0; i < sn.Categories[category_id].Items.Length; i++)
            {
                if (sn.Categories[category_id].Items[i].AR == 1)
                {
                    items.Add(sn.Categories[category_id].Items[i]);
                }
            }

            Debug.Log("Order:" + android_order);
            Debug.Log("Clear:" + clear);
            Debug.Log("Category_id in focus:" + category_id);

            dropDownoptions = new List<string>();
            for (int j = 0; j < sn.Categories.Length; j++)
            {
                int f = 0;
                //Debug.Log("Category:" + sn.Restaurants[restaurant_id].Categories[i].Category_name);

                for (int i = 0; i < sn.Categories[j].Items.Length; i++)
                {
                    if (sn.Categories[j].Items[i].AR == 1)
                    {
                        f = 1;
                        break;
                    }
                }
                if (f == 1)
                {
                    dropDownoptions.Add(sn.Categories[j].Category_name);
                }

            }


            drop.ClearOptions();
            drop.AddOptions(dropDownoptions);

            drop.RefreshShownValue();






            //AssetBundle Fetching & Caching
            StartCoroutine(DownloadAndCache());

          

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

        new NativeShare().AddFile(filePath).SetSubject("Food").SetText("Checkout the " + assetName + " at " + sn.Name).Share();


    }

    public void loadPrevious()
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
       
        currentActivity.Call("onBackAndroid2");
    }

    public IEnumerator categoryChanged()
    {
        //category_id = drop.value;
        Debug.Log("id: " + category_id);


        if (bundle != null)
        {
            bundle.Unload(false);
        }

        for (int i = 0; i < bundleSize; i++)
        {
            Destroy(mButtonInstance[i]);
        }
        //flag = 0;
        //attachObject();
        //LoadThis(0);
        //Destroy(mBundleInstance);
        if (mBundleInstance)
        {
            Destroy(mBundleInstance);

        }
        items = new List<Items>();
        for (int i = 0; i < sn.Categories[category_id].Items.Length; i++)
        {
            if (sn.Categories[category_id].Items[i].AR == 1)
            {
                items.Add(sn.Categories[category_id].Items[i]);
            }
        }
        dropDownoptions = new List<string>();
        for (int j = 0; j < sn.Categories.Length; j++)
        {
            int f = 0;
            //Debug.Log("Category:" + sn.Restaurants[restaurant_id].Categories[i].Category_name);

            for (int i = 0; i < sn.Categories[j].Items.Length; i++)
            {
                if (sn.Categories[j].Items[i].AR == 1)
                {
                    f = 1;
                    break;
                }
            }
            if (f == 1)
            {
                dropDownoptions.Add(sn.Categories[j].Category_name);
            }

        }


        drop.ClearOptions();
        drop.AddOptions(dropDownoptions);

        drop.RefreshShownValue();

        Debug.Log("Beforee");
        Debug.Log("mAttached in download: " + mAttached);
        if (flag == 1)
        {
            //changed on 22/06/19
            Debug.Log("In Download");

            //Destroy(mBundleInstance);

            // mBundleInstance.SetActive(false);
            //  mAttached = false;
            if (bundle != null)
            {
                bundle.Unload(false);
            }


        }

        if (BundleURL != "")
        {
            // Wait for the Caching system to be ready

            while (!Caching.ready)

                yield return null;



            loadingpanel.SetActive(true);


            // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
            //Get the bundlelink from firebase object and also get version code in case of any update
            //Thus whenever you upload different  assetbundle with same link, change the version code in Firebase to download & cache it again
            www = WWW.LoadFromCacheOrDownload(sn.Categories[category_id].Category_bundle_link + "", sn.Categories[category_id].Version.GetHashCode());




            var FilePath = Application.streamingAssetsPath;
            //Configure progress UI
            while (!www.isDone)
            {
                progressBar.text = "Loading  " + Math.Round(www.progress * 100) + "%";

                yield return null;

            }


            progressBar.text = "Loading Completed";
            //Once loaded disable the LoadingPanel GameObject
            loadingpanel.SetActive(false);

            if (!string.IsNullOrEmpty(www.error))
            {

                throw new Exception("WWW download had an error:" + www.error);
            }

            else
            {

                bundle = www.assetBundle;

                ItemNames = new string[bundle.GetAllAssetNames().Length];

                bundleSize = bundle.GetAllAssetNames().Length;
                // Debug.Log("Size:" + bundleSize);


                if (AssetName == "")
                {
                    mBundleInstance = (Instantiate(bundle.mainAsset)) as GameObject;
                }

                else
                {


                    currentAsset = 0;
                    Debug.Log("initialisng of bundle in category changed");
                    mBundleInstance = (Instantiate(bundle.LoadAsset(bundle.GetAllAssetNames()[0]))) as GameObject;
                    mBundleInstance.SetActive(false);
                }

                assetName = items[currentAsset].Name;
                //assetName = sn.Restaurants[restaurant_id].Categories[category_id].Items[currentAsset].Name;
                Assename.text = assetName;

                Price.text = "₹" + items[currentAsset].Price.ToString();
                Description.text = items[currentAsset].Description.ToString();
                // Price.text = "₹" + sn.Restaurants[restaurant_id].Categories[category_id].Items[currentAsset].Price.ToString();
                // Description.text = sn.Restaurants[restaurant_id].Categories[category_id].Items[currentAsset].Description.ToString();

                mButtonInstance = new GameObject[bundleSize];
                for (int i = 0; i < bundleSize; i++)
                {

                    int j = i;
                    if (flag == 1)
                    {
                        Destroy(mButtonInstance[i]);

                    }
                    mButtonInstance[i] = Instantiate(mContentPrefab, mContent.transform, false);
                    mButtonInstance[i].GetComponentInChildren<Text>().text = bundle.LoadAsset(bundle.GetAllAssetNames()[i]).name;
                    // mButtonInstance[i].GetComponentInChildren<Text>().text = sn.Restaurants[restaurant_id].Categories[category_id].Items[i].Name;
                    //   Debug.Log("Name"+ sn.Restaurants[restaurant_id].Categories[category_id].Items[i].Name.ToUpper());
                    mButtonInstance[i].GetComponent<Button>().onClick.AddListener(delegate
                    {

                        LoadThis(j);
                    });
                }

                mExpandedView.SetActive(false);




            }
        }
        else
        {



        }
        mAttached = false;

        //LoadNext();

    }
    public void callDownload()
    {
        //category_id = drop.value;

        for (int i = 0; i < sn.Categories.Length; i++)
        {
            if (sn.Categories[i].Category_name.Equals(drop.options[drop.value].text))
            {
                category_id = i;
                break;
            }
        }
        Debug.Log("Category_id in call download:" + category_id);
        StartCoroutine(categoryChanged());
    }
    IEnumerator DownloadAndCache()
    {
        Debug.Log("mAttached in download: " + mAttached);
        if (flag == 1)
        {
            //changed on 22/06/19
            Debug.Log("In Download");

            //Destroy(mBundleInstance);

            // mBundleInstance.SetActive(false);
            //  mAttached = false;
            //if (bundle != null)
            //{
            //    bundle.Unload(false);
            //}


        }

        if (BundleURL != "")
        {
            // Wait for the Caching system to be ready

            while (!Caching.ready)

                yield return null;



            loadingpanel.SetActive(true);
           

            // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
            //Get the bundlelink from firebase object and also get version code in case of any update
            //Thus whenever you upload different  assetbundle with same link, change the version code in Firebase to download & cache it again
            www = WWW.LoadFromCacheOrDownload(sn.Categories[category_id].Category_bundle_link + "", sn.Categories[category_id].Version.GetHashCode());

           


            var FilePath = Application.streamingAssetsPath;
            Debug.Log("URL: " + sn.Categories[category_id].Category_bundle_link);
            //Configure progress UI
            while (!www.isDone)
            {
                progressBar.text = "Loading  " + Math.Round(www.progress * 100) + "%";
              

                yield return null;

            }
          

            progressBar.text = "Loading Completed";
            //Once loaded disable the LoadingPanel GameObject
            loadingpanel.SetActive(false);

            if (!string.IsNullOrEmpty(www.error))
            {

                throw new Exception("WWW download had an error:" + www.error);
            }

            else
            {

                bundle = www.assetBundle;

                ItemNames = new string[bundle.GetAllAssetNames().Length];

                bundleSize = bundle.GetAllAssetNames().Length;
                // Debug.Log("Size:" + bundleSize);


                if (AssetName == "")
                {
                    mBundleInstance = (Instantiate(bundle.mainAsset)) as GameObject;
                }

                else
                {
                    for (int i = 0; i < bundleSize; i++)
                    {
                        Debug.Log("ItemName:" + items[i].Name);
                        Debug.Log("ItemName:" + sn.Categories[category_id].Items[currentAsset].Name);
                        if (sn.Categories[category_id].Items[currentAsset].Name.Equals(items[i].Name))
                        {
                            Debug.Log("ItemName:" + items[i].Name);
                            currentAsset = i;
                            break;
                        }

                    }

                    // currentAsset = 0;
                    Debug.Log("Current Asset in Download:" + currentAsset);
                    Debug.Log("initialisng of bundle");
                    mBundleInstance = (Instantiate(bundle.LoadAsset(bundle.GetAllAssetNames()[currentAsset]))) as GameObject;
                    mBundleInstance.SetActive(false);
                }

                assetName = items[currentAsset].Name;
                //assetName = sn.Restaurants[restaurant_id].Categories[category_id].Items[currentAsset].Name;
                Assename.text = assetName;

                Price.text = "₹" + items[currentAsset].Price.ToString();
                Description.text = items[currentAsset].Description.ToString();

                mButtonInstance = new GameObject[bundleSize];
                for (int i = 0; i < bundleSize; i++)
                {

                    int j = i;
                    if (flag == 1)
                    {
                        Destroy(mButtonInstance[i]);

                    }
                    mButtonInstance[i] = Instantiate(mContentPrefab, mContent.transform, false);
                    mButtonInstance[i].GetComponentInChildren<Text>().text = bundle.LoadAsset(bundle.GetAllAssetNames()[i]).name;
                    // mButtonInstance[i].GetComponentInChildren<Text>().text = sn.Restaurants[restaurant_id].Categories[category_id].Items[i].Name;
                    //   Debug.Log("Name"+ sn.Restaurants[restaurant_id].Categories[category_id].Items[i].Name.ToUpper());
                    mButtonInstance[i].GetComponent<Button>().onClick.AddListener(delegate
                    {

                        LoadThis(j);
                    });
                }

                mExpandedView.SetActive(false);




            }
        }
        else
        {



        }
    }

    public void LoadNext()
    {

        Debug.Log("Inside next");
        Destroy(mBundleInstance);



        currentAsset = (currentAsset + 1) % bundleSize;
        mBundleInstance = Instantiate(bundle.LoadAsset(bundle.GetAllAssetNames()[currentAsset])) as GameObject;
        //  assetName = mBundleInstance.name;
        //assetName = sn.Restaurants[restaurant_id].Categories[category_id].Items[currentAsset].Name;
        mBundleInstance.SetActive(false);
        assetName = items[currentAsset].Name;
        //assetName = sn.Restaurants[restaurant_id].Categories[category_id].Items[currentAsset].Name;
        Assename.text = assetName;

        Price.text = "₹" + items[currentAsset].Price.ToString();
        Description.text = items[currentAsset].Description.ToString();

        mAttached = false;



    }
    public void LoadPrev()
    {


        Destroy(mBundleInstance);


        if (currentAsset == 0)
            currentAsset = bundleSize - 1;
        else
            currentAsset = (currentAsset - 1) % bundleSize;
        mBundleInstance = Instantiate(bundle.LoadAsset(bundle.GetAllAssetNames()[currentAsset])) as GameObject;

        // assetName = sn.Restaurants[restaurant_id].Categories[category_id].Items[currentAsset].Name;
        mBundleInstance.SetActive(false);
        assetName = items[currentAsset].Name;
        //assetName = sn.Restaurants[restaurant_id].Categories[category_id].Items[currentAsset].Name;
        Assename.text = assetName;

        Price.text = "₹" + items[currentAsset].Price.ToString();
        Description.text = items[currentAsset].Description.ToString();
        mAttached = false;

    }
    public void attachObject()
    {

        Debug.Log("Inside attach");
        mBundleInstance.transform.SetParent(transform,true);
        mBundleInstance.transform.localRotation = Quaternion.Euler(-120f, 0, 0);
        mBundleInstance.transform.localPosition = new Vector3(0.0f, 0.0f, 20.0f);
        mBundleInstance.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        mBundleInstance.AddComponent<touchEvents>();
        mBundleInstance.transform.gameObject.SetActive(true);

        mAttached = true;

    }
    void ActivateExpandedView()
    {
        if (mExpand.GetComponent<UnityEngine.UI.Image>().sprite == showExpanded)
            mExpand.GetComponent<UnityEngine.UI.Image>().sprite = hideExpanded;
        else
            mExpand.GetComponent<UnityEngine.UI.Image>().sprite = showExpanded;

        mExpandedView.SetActive(!mExpandedView.activeSelf);


    }
    void LoadThis(int idx)
    {

        Destroy(mBundleInstance);



        currentAsset = idx;
        mBundleInstance = Instantiate(bundle.LoadAsset(bundle.GetAllAssetNames()[currentAsset])) as GameObject;
        //assetName = sn.Restaurants[restaurant_id].Categories[category_id].Items[currentAsset].Name;
        mBundleInstance.SetActive(false);
        //Assename.text = assetName;
        assetName = items[currentAsset].Name;
        //assetName = sn.Restaurants[restaurant_id].Categories[category_id].Items[currentAsset].Name;
        Assename.text = assetName;

        Price.text = "₹" + items[currentAsset].Price.ToString();
        Description.text = items[currentAsset].Description.ToString();


        mExpandedView.SetActive(false);
        mExpand.GetComponent<UnityEngine.UI.Image>().sprite = showExpanded;
        mAttached = false;


    }

    void ShowToast(string username, int flag)
    {


        GameObject.Find("Popup").transform.GetChild(0).gameObject.GetComponent<Text>().text = username;
        GameObject.Find("Popup").transform.localScale = new Vector2(1f, 1f);
        StartCoroutine(wait(flag));

    }

    IEnumerator wait(int flag)
    {
        yield return new WaitForSeconds(40f);
        try
        {
            GameObject.Find("Popup").transform.localScale = new Vector2(1f, 0f);
            if (flag == 1) { SceneManager.LoadScene("Login"); }
        }
        catch
        {
            Debug.Log("Popup Missed");
        }
    }

    // Update is called once per frame
    void Update()
    {

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

            currentActivity.Call("onBackAndroid2");
        }

        if (progressBar.text.Equals("Loading Completed") && !mAttached)
        {
           
            attachObject();
            Debug.Log("Inside track");
        }
    }
}

