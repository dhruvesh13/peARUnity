using UnityEngine;
using UnityEditor;
public class BuildAssetBundles{

[MenuItem("Assets/ Build Assets")]
static void buildAsset(){

BuildPipeline.BuildAssetBundles("Assets/Bundle",BuildAssetBundleOptions.ChunkBasedCompression,BuildTarget.Android);

}


}