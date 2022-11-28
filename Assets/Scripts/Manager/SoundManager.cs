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
    Portion,
    Cheap,
    Expensive,
    PlayerDash,
    PlayerAttack_Normal,
    PlayerAttack_Fire,
    PlayerAttack_Electric,
    Boss,
    BGM,
    GameOver,
}

public class SoundManager : MonoBehaviour
{

    //Bgm, Effect 두 개를 source로 들고 있을 것
    private List<AudioSource> _audioSources = new List<AudioSource>();
    
    [SerializeField]
    private List<AudioClip> _Boxclips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> _Doorclips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> _Zombieclips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> _RushZombieclips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> _RevivalZombieclips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> _Portionclips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> _Coinclips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> _PlayerDashclips = new List<AudioClip>();

    [SerializeField] 
    private List<AudioClip> _PlayerAttackclips = new List<AudioClip>();
    
    [SerializeField]
    private List<AudioClip> _GameOverclips = new List<AudioClip>();
    
    [SerializeField]
    private List<AudioClip> _BGMclips = new List<AudioClip>();

    private int i;
    public float bgmvolume, effectvolume, totalvolume;

    private static SoundManager _instance = null;

    public static SoundManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            // TODO - 임시코드!!! 수정 필요
            DontDestroyOnLoad(this.gameObject);
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
            root.AddComponent<BGMPlayer>();
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

            totalvolume = bgmvolume = effectvolume = 1;
        }
    }

    //좀비 사운드를 넣어준다
    public AudioClip[] ZombieClip(MonsterType type = MonsterType.Zombie)
    {
        AudioClip[] clip = new AudioClip[3];

        switch (type)
        {
            case MonsterType.RushZombie:
                int rand = Random.Range(0, 1) * 3;
                clip[0] = _RushZombieclips[rand];
                clip[1] = _RushZombieclips[rand + 1];
                clip[2] = _RushZombieclips[rand + 2];
                break;

            case MonsterType.RevivalZombie:
                clip[0] = _RevivalZombieclips[0];
                clip[1] = _RevivalZombieclips[1];
                clip[2] = _RevivalZombieclips[2];
                break;

            default:
                clip[1] = _Zombieclips[Random.Range(0, 1) * 2];
                clip[2] = _Zombieclips[1];
                break;
        }

        return clip;
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

    public void SoundPlay(SoundType st, Sound type = Sound.Effect, int index = 0, float pitch = 1.0f)
    {
        switch (st)
        {
            case SoundType.BGM:
                Play(_BGMclips[index], Sound.Bgm);
                break;

            case SoundType.Box:
                Play(_Boxclips[Random.Range(0, _Boxclips.Count)]);
                break;

            case SoundType.DoorClose:
                Play(_Doorclips[0]);
                break;

            case SoundType.DoorOpen:
                Play(_Doorclips[1]);
                break;

            case SoundType.Portion:
                Play(_Portionclips[0]);
                break;

            case SoundType.Cheap:
                Play(_Coinclips[0]);
                break;

            case SoundType.Expensive:
                Play(_Coinclips[1]);
                break;

            case SoundType.PlayerDash:
                Play(_PlayerDashclips[0]);
                break;
            
            case SoundType.PlayerAttack_Normal:
                print(_PlayerAttackclips.Count);
                Play(_PlayerAttackclips[0]);
                break;
            
            case SoundType.PlayerAttack_Fire:
                Play(_PlayerAttackclips[1]);
                break;
            
            case SoundType.PlayerAttack_Electric:
                Play(_PlayerAttackclips[2]);
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
