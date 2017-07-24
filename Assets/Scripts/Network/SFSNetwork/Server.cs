using UnityEngine;
using System.Collections;

public class Server{
    public int id;
    public string name;
    public string ip;
    public int state;
    public int type;
    public int load;
    public string region;
    public int queue;
    public int user_count;

    public Server(string str)
    {
        if (str != string.Empty)
        {
            string[] strs = str.Split(","[0]);
            foreach (string s in strs)
            {
                string[] keyValue = s.Split(":"[0]);
                string key = keyValue[0];
                string value = keyValue[1];

                if (key == "id")
                    id = int.Parse(value);
                else if (key == "name")
                    name = value;
                else if (key == "ip")
                    ip = value;
                else if (key == "state")
                    state = int.Parse(value);
                else if (key == "type")
                    type = int.Parse(value);
                else if (key == "load")
                    load = int.Parse(value);
                else if (key == "region")
                    region = value;
                else if (key == "queue")
                    queue = int.Parse(value);
                else if (key == "user_count")
                    user_count = int.Parse(value);
            }
                
        }
    }
}
