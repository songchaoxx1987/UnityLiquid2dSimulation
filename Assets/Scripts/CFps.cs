using UnityEngine;

static class CFps
{
    private static float m_LastUpdateShowTime = 0f;  //上一次更新帧率的时间;  

    private static float m_UpdateShowDeltaTime = 1.0f;//更新帧率的时间间隔;  

    private static int m_FrameUpdate = 0;//帧数;  

    private static float m_FPS = 0;
    static GUIStyle style = new GUIStyle();
    public static float FPS { get { return m_FPS; } }
    public static void Start()
    {
        m_LastUpdateShowTime = Time.realtimeSinceStartup;
        Application.targetFrameRate = 100;
        //style.fontSize = 50;
    }

    public static void Update()
    {
        m_FrameUpdate++;
        if (Time.realtimeSinceStartup - m_LastUpdateShowTime >= m_UpdateShowDeltaTime)
        {
            m_FPS = m_FrameUpdate / (Time.realtimeSinceStartup - m_LastUpdateShowTime);
            m_FrameUpdate = 0;
            m_LastUpdateShowTime = Time.realtimeSinceStartup;
        }
    }
        
    public static void OnGUI()
    {
        GUI.skin.label.fontSize = 30;
        int w = Screen.width / 10;
        int h = Screen.height / 10;
        if (m_FPS >= 30.0f)
        {
            GUI.skin.label.normal.textColor = Color.green;
        }
        else if (m_FPS >= 20.0f)
        {
            GUI.skin.label.normal.textColor = Color.yellow;
        }
        else
        {
            GUI.skin.label.normal.textColor = Color.red;
        }       
        GUI.Label(new Rect(w * 0.5f, h * 0.5f, 200, 100), "FPS: " + m_FPS.ToString("f2"));
        GUI.skin.label.normal.textColor = Color.red;
//        GUI.Label(new Rect(w * 0.5f, h * 0.5f+110, 200, 100), "Grp: " + TestSwitch.instance.Group);
    }

}

