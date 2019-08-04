using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaveSystem : MonoBehaviour {

	public enum SpawnState { SPAWNING, WAITING, COUNTING };

	[System.Serializable]
	public class Wave
	{
		public string name;
		public Transform enemyOne;
        public Transform enemyTwo;
        public Transform enemyThree;
        public Transform enemyFour;
        public Transform enemyFive;
        public int count;
		public float rate;
	}

    IEnumerator WaveCompletedMessage(string message, float delay)
    {
        waveCompleted.text = message;
        waveCompleted.enabled = true;
        yield return new WaitForSeconds(delay);
        waveCompleted.enabled = false;
    }

    IEnumerator WaveIncomingMessage(string message, float delay)
    {
        spawningWave.text = message;
        spawningWave.enabled = true;
        yield return new WaitForSeconds(delay);
        spawningWave.enabled = false;
    }

    public Text waveNumber;
    public Text gameOverWaveNumber;
    public Text waveCompleted;
    public Text spawningWave;

    public int waveCount;

	public Wave[] waves;
	private int nextWave = 0;
	public int NextWave
	{
		get { return nextWave + 1; }
	}

	public Transform[] spawnPoints;

	public float timeBetweenWaves = 5f;
	private float waveCountdown;
	public float WaveCountdown
	{
		get { return waveCountdown; }
	}

	private float searchCountdown = 1f;

	private SpawnState state = SpawnState.COUNTING;
	public SpawnState State
	{
		get { return state; }
	}

	void Start()
	{
		if (spawnPoints.Length == 0)
		{
			Debug.LogError("No spawn points referenced.");
		}

		waveCountdown = timeBetweenWaves;
	}

	void Update()
	{
		if (state == SpawnState.WAITING)
		{
			if (!EnemyIsAlive())
			{
				WaveCompleted();
                waveCount++;

                waveNumber.text = "" + waveCount;
                gameOverWaveNumber.text = "" + waveCount;
            }
			else
			{
				return;
			}
		}

		if (waveCountdown <= 0)
		{
			if (state != SpawnState.SPAWNING)
			{
				StartCoroutine( SpawnWave ( waves[nextWave] ) );
			}
		}
		else
		{
			waveCountdown -= Time.deltaTime;
		}
	}

	void WaveCompleted()
	{
		Debug.Log("Wave Completed!");

        StartCoroutine(WaveCompletedMessage("Wave Completed!", 3));

        state = SpawnState.COUNTING;
		waveCountdown = timeBetweenWaves;

		if (nextWave + 1 > waves.Length - 1)
		{
			nextWave = 0;
                                 
			Debug.Log("ALL WAVES COMPLETE! Looping...");
		}
		else
		{
			nextWave++;
		}
	}

	bool EnemyIsAlive()
	{
		searchCountdown -= Time.deltaTime;
		if (searchCountdown <= 0f)
		{
			searchCountdown = 1f;
			if (GameObject.FindGameObjectWithTag("Enemy") == null)
			{
				return false;
			}
		}
		return true;
	}

	IEnumerator SpawnWave(Wave _wave)
	{
		Debug.Log("Spawning Wave: " + _wave.name);

        StartCoroutine(WaveIncomingMessage("New Wave Incoming \nGet Ready!", 3));

        state = SpawnState.SPAWNING;

		for (int i = 0; i < _wave.count; i++)
		{
			SpawnEnemy(_wave.enemyOne);
            SpawnEnemy(_wave.enemyTwo);
            SpawnEnemy(_wave.enemyThree);
            SpawnEnemy(_wave.enemyFour);
            SpawnEnemy(_wave.enemyFive);
            yield return new WaitForSeconds( 1f/_wave.rate );
		}

		state = SpawnState.WAITING;

		yield break;
	}

	void SpawnEnemy(Transform _enemy)
	{
		Debug.Log("Spawning Enemy: " + _enemy.name);

		Transform _sp = spawnPoints[ Random.Range (0, spawnPoints.Length) ];
		Instantiate(_enemy, _sp.position, _sp.rotation);
	}

}
