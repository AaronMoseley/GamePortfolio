using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class PlayerSaveSystem : MonoBehaviour
{
    public float waitTime;

    GameObject player;
    ItemList itemList;

    bool loading = false;
    int correctScene;

    public GameObject overrideMenu;
    public InGameMenuManager menu;

    public int currOverriding;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        correctScene = SceneManager.GetActiveScene().buildIndex;

        player = GameObject.FindGameObjectWithTag("Player");
        itemList = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<ItemList>();
        menu = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InGameMenuManager>();
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex != correctScene && !loading)
        {
            Destroy(gameObject);
        }
    }

    public void TestCanSave(int saveNum)
    {
        if(PlayerSaveLoad.LoadPlayer(saveNum) == null)
        {
            Save(saveNum);
            currOverriding = -1;
        } else
        {
            overrideMenu.SetActive(true);
            currOverriding = saveNum;
        }
    }

    public void Save(int saveNum)
    {
        overrideMenu.SetActive(false);
        menu.ShowSaves(false);
        
        if (saveNum == -1)
        {
            saveNum = currOverriding;
        }
        
        currOverriding = -1;
        player = GameObject.FindGameObjectWithTag("Player");
        Inventory inventory = GameObject.FindGameObjectWithTag("Inventory Manager").GetComponent<Inventory>();
        HotkeyManager hotkeyManage = GameObject.FindGameObjectWithTag("Inventory Manager").GetComponent<HotkeyManager>();

        string[] slots = new string[inventory.invSlots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            if (inventory.invSlots[i].GetComponent<InvSlot>().item != null)
            {
                slots[i] = inventory.invSlots[i].GetComponent<InvSlot>().item.itemID;
            }
            else
            {
                slots[i] = "";
            }
        }

        int[] hotkeys = new int[hotkeyManage.hotkeyTargets.Count];

        for (int i = 0; i < hotkeys.Length; i++)
        {
            if (hotkeyManage.hotkeyTargets[i] != null)
            {
                for (int j = 0; j < inventory.invSlots.Length; j++)
                {
                    if (hotkeyManage.hotkeyTargets[i] == inventory.invSlots[j].GetComponent<InvSlot>())
                    {
                        hotkeys[i] = j;
                    }
                }
            }
            else
            {
                hotkeys[i] = -1;
            }
        }

        PlayerSaveLoad.SaveData(player.transform.position, slots, hotkeys, player.GetComponent<PlayerHealth>().currHealth, SceneManager.GetActiveScene().buildIndex, saveNum);
    }

    public void Load(int saveNum)
    {
        Time.timeScale = 1;
        PlayerData data = PlayerSaveLoad.LoadPlayer(saveNum);

        loading = true;
        SceneManager.LoadScene(data.currScene);

        StartCoroutine(LoadPlayer(data));
    }

    IEnumerator LoadPlayer(PlayerData data)
    {
        yield return new WaitForSeconds(waitTime);
        player = GameObject.FindGameObjectWithTag("Player");
        itemList = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<ItemList>();
        Inventory inventory = player.GetComponentInChildren<Canvas>().gameObject.GetComponentInChildren<Inventory>();
        HotkeyManager hotkeyManage = player.GetComponentInChildren<Canvas>().gameObject.GetComponentInChildren<HotkeyManager>();

        player.GetComponent<PlayerHealth>().AddHealth(player.GetComponent<PlayerHealth>().maxHealth);

        for(int i = 0; i < player.GetComponent<PlayerHealth>().maxHealth - data.health; i++)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(1);
        }

        player.transform.position = new Vector3(data.xPos, data.yPos, data.zPos);

        for (int i = 0; i < data.items.Length; i++)
        {
            inventory.invSlots[i].GetComponent<InvSlot>().ResetItem();

            if (data.items[i] != "")
            {
                for (int j = 0; j < itemList.items.Count; j++)
                {
                    if (itemList.items[j].itemID == data.items[i])
                    {
                        inventory.invSlots[i].GetComponent<InvSlot>().AddItem(itemList.items[j]);
                    }
                }
            }
        }

        for (int i = 1; i < data.hotkeyItems.Length; i++)
        {
            if (data.hotkeyItems[i] != -1)
            {
                hotkeyManage.hotkeyTargets[i] = player.GetComponentInChildren<Inventory>().invSlots[data.hotkeyItems[i]].GetComponent<InvSlot>();
                hotkeyManage.hotkeySlots[i].GetComponent<Image>().sprite = player.GetComponentInChildren<Inventory>().invSlots[data.hotkeyItems[i]].GetComponent<InvSlot>().item.itemIcon;
                hotkeyManage.hotkeySlots[i].GetComponent<Image>().enabled = true;
            }
        }

        Destroy(gameObject);
    }
}
