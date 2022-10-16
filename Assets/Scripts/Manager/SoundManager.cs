using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sound
{
    Effect,
    Bgm
}

public enum SoundType
{
    Box,
    DoorClose,
    DoorOpen,
    Monster,
    Boss
}

public class SoundManager : MonoBehaviour
{
    //Bgm, Effect 두 개를 source로 들고 있을 것
    private List<AudioSource> _audioSources = new List<AudioSource>();
    
    [SerializeField]
    private List<AudioClip> _Boxclips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> _Doorclips = new List<AudioClip>();

    private int i;
    public float bgmvolume, effectvolume, totalvolume;

    private static SoundManager _instance = null;

    public static SoundManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        GameObject root = GameObject.Find("@Sound");

        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Sound));

            for (int i = 0; i < soundNames.Length; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources.Add(go.AddComponent<AudioSource>());
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Sound.Bgm].loop = true;
            if (_Boxclips == null)
                BoxClip();
            if(_Doorclips == null)
                DoorClip();
        }
    }

    //만약 박스 클립이 할당되지 않을 경우
    private void BoxClip()
    {
        for (i = 1; i < 6; i++)
            _Boxclips.Add(Resources.Load<AudioClip>($"Sounds/Effect/Box/Box {i}"));
    }

    //만약 도어 클립이 할당되지 않을 경우
    private void DoorClip()
    {
        _Doorclips.Add(Resources.Load<AudioClip>("Sounds/Effect/Door/Door 1 Close"));
        _Doorclips.Add(Resources.Load<AudioClip>("Sounds/Effect/Door/Door 3 Open"));
    }

    public void SoundPlay(SoundType st, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        switch (st)
        {
            case SoundType.Box:
                Play(_Boxclips[Random.Range(0, _Boxclips.Count)]);
                break;

            case SoundType.DoorClose:
                Play(_Doorclips[0]);
                break;

            case SoundType.DoorOpen:
                Play(_Doorclips[1]);
                break;

            case SoundType.Monster:
                break;
        }
    }

    public void Play(AudioClip audioClip, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        if (type == Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Sound.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.volume = bgmvolume * totalvolume;
            audioSource.Play();
        }

        else
        {
            AudioSource audioSource = _audioSources[(int)Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.volume = effectvolume * totalvolume;
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void SetBgmVolume()
    {
        AudioSource audioSource = _audioSources[(int)Sound.Bgm];
        audioSource.volume = bgmvolume * totalvolume;
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
    }
}
