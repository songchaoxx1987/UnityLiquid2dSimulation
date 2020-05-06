using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class LiquidParticleEmitter
{
    public const int maxCount = 1024;
    public GameObject particle = null;
    public List<GameObject> particleList = new List<GameObject>(maxCount);
    public Matrix4x4[] matrices = new Matrix4x4[maxCount];

    public bool enable = false;
    public float interval = 0.01f;

    public Vector3 srcPos = new Vector3();
    public Vector2 force = new Vector2(1, 0);
    public float timePass = 0;
}

public class LiquidCmdPostProcess : MonoBehaviour
{
    static LiquidCmdPostProcess _instance = null;

    public static LiquidCmdPostProcess instance { get { return _instance; } }

    public Camera mainCamera = null;    
    public Material matMetaBall = null;
    public RenderTexture rt = null;
    CommandBuffer cmd = null;
    
    
    public List<LiquidParticleEmitter> liquidParticleEmitters = new List<LiquidParticleEmitter>();

    void Awake()
    {
        _instance = this;
        mainCamera.depthTextureMode |= DepthTextureMode.Depth;
        _Setup();

        LiquidParticleEmitter e = new LiquidParticleEmitter();
        e.srcPos = new Vector3(1.42f, 4, 0);
        e.particle = Resources.Load<GameObject>("prefab/particle_blue");
        e.enable = true;
        e.force = new Vector2(-100, 0);
        liquidParticleEmitters.Add(e);

        e = new LiquidParticleEmitter();
        e.srcPos = new Vector3(-1.42f, 4, 0);
        e.particle = Resources.Load<GameObject>("prefab/particle_pink");
        e.enable = true;
        e.force = new Vector2(100, 0);
        liquidParticleEmitters.Add(e);
    }


    void _Setup()
    {
        cmd = new CommandBuffer();

        int w = Screen.width;
        int h = Screen.height;
        if (w > SystemInfo.maxTextureSize)
        {
            h = h * SystemInfo.maxTextureSize / w;
            w = SystemInfo.maxTextureSize;
        }
        if (h > SystemInfo.maxTextureSize)
        {
            w = w * SystemInfo.maxTextureSize / h;
            h = SystemInfo.maxTextureSize;
        }
        rt = RenderTexture.GetTemporary(w, h);

        cmd.SetRenderTarget(rt);
        matMetaBall.SetTexture("_MetaBallTex", rt);
        matMetaBall.SetPass(0);
        mainCamera.AddCommandBuffer(CameraEvent.BeforeImageEffects, cmd);
    }

    void _Release()
    {
        mainCamera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, cmd);
    }

    private void Update()
    {
        var dt = Time.deltaTime;
        for(int i = 0; i < liquidParticleEmitters.Count; ++i)
        {
            var e = liquidParticleEmitters[i];
            e.timePass += dt;
            if (e.timePass >= e.interval &&
                e.particleList.Count< LiquidParticleEmitter.maxCount)
            {
                var go = GameObject.Instantiate(e.particle);
                go.transform.position = e.srcPos;
                e.particleList.Add(go);
                var r = go.GetComponent<Rigidbody2D>();
                r.AddForce(e.force);
                e.timePass = 0;
            }
            
            int k = 0;
            for (int j = e.particleList.Count-1; j>= 0; --j)
            {
                var obj = e.particleList[j];
                if (obj.transform.position.y <= 0)
                {
                    e.particleList.RemoveAt(j);
                    GameObject.Destroy(obj);
                }
                else
                {
                    e.matrices[k] = obj.transform.localToWorldMatrix;
                    ++k;
                }
            }
        }
    }

    void OnPostRender()
    {        
        cmd.Clear();
        cmd.SetRenderTarget(rt);
        cmd.ClearRenderTarget(true, true, Color.clear);
        for(int i = 0; i < liquidParticleEmitters.Count; ++i)
        {
            var e = liquidParticleEmitters[i];
            var obj = e.particle;
            var r = obj.GetComponent<Renderer>();
            var mf = obj.GetComponent<MeshFilter>();
            cmd.DrawMeshInstanced(mf.sharedMesh, 0, r.sharedMaterial, 0, e.matrices, e.particleList.Count);
        }        
    }
    
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        
        Graphics.Blit(source, destination, matMetaBall);
    }


    private void OnDestroy()
    {
        _Release();
        if (rt != null)
        {
            RenderTexture.ReleaseTemporary(rt);
            rt = null;
        }
    }


}
