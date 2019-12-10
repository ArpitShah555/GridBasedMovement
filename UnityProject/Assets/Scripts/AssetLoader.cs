using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AssetLoader : MonoBehaviour {

    public Text debugTxt;

    IEnumerator Start() {
        string bundlePath = "file://"+Application.persistentDataPath + Path.DirectorySeparatorChar + "playercharacter";
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(bundlePath);
        yield return www.SendWebRequest();
        if (www.error != null) {
            debugTxt.text = www.error+"\n\n\n\nAsset Bundle Not Found at:\n\n" + bundlePath + "\n\n\n\nApplication will now Quit";
            yield return new WaitForSeconds(5f);
            Application.Quit();
        } else {
            // load the player model from the asset bundle
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            AssetBundleRequest loadAsset = bundle.LoadAssetAsync<GameObject>("Assets/Player/Player.prefab");
            yield return loadAsset;
            GetComponent<GridGenerator>().playerPrefab = loadAsset.asset as GameObject;
            debugTxt.gameObject.SetActive(false);
        }
    }
}
