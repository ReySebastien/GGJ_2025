using UnityEngine;
using UnityEngine.Pool;

namespace GGJ
{
    public class BubblePool : CustomBehaviour
    {
        [SerializeField] private PickableBubble _bubblePrefab; // Le prefab de la bulle
        [SerializeField] private Transform _poolParent; //Parent des bulles dans la hi�rarchie
        [SerializeField] private int _initialPoolSize = 0; // Taille initiale du pool
        [SerializeField] private int _maxPoolSize = 0; // Taille maximale du pool

        private ObjectPool<PickableBubble> _pool;
        private int _totalObjects = 0; // Nombre d'objets actuellement actifs + en pool


        private void Awake()
        {

            // V�rifie si un parent a �t� d�fini, sinon cr�e-en un
            if (_poolParent == null)
            {
                GameObject poolParentObj = new GameObject("BubblePool");
                _poolParent = poolParentObj.transform;
            }

            // Initialisation du pool avec des fonctions pour la cr�ation, l'activation, la d�sactivation et la destruction des objets
            _pool = new ObjectPool<PickableBubble>(
                CreatePooledItem,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                true,
                _initialPoolSize,
                _maxPoolSize
            );

            // Pr�-remplir le pool jusqu'� sa taille initiale
            for (int i = 0; i < _initialPoolSize; i++)
            {
                var bubble = CreatePooledItem();
                _pool.Release(bubble); // Retourne imm�diatement au pool
            }

            _totalObjects = _initialPoolSize; // Initialiser avec la taille initiale
        }

        private PickableBubble CreatePooledItem()
        {
            if (_totalObjects >= _maxPoolSize)
            {
                // Ne pas cr�er de nouvelles bulles si on a atteint la limite
                Debug.LogWarning("Max pool size reached! Cannot create more objects.");
                return null;
            }

            // Cr�ation d'une nouvelle bulle
            var bubble = Instantiate(_bubblePrefab, _poolParent);
            bubble.GetComponent<PickableBubble>().SetPool(_pool);
            _totalObjects++; // Incr�menter le total
            return bubble;
        }

        private void OnTakeFromPool(PickableBubble bubble)
        {
            // Activation de la bulle lorsqu'elle est prise du pool
            bubble.gameObject.SetActive(true);
        }
        private void OnReturnedToPool(PickableBubble bubble)
        {
            // D�sactivation de la bulle lorsqu'elle est retourn�e au pool
            bubble.gameObject.SetActive(false);
        }


        private void OnDestroyPoolObject(PickableBubble bubble)
        {
            // Destruction de la bulle lorsque le pool est nettoy�
            Destroy(bubble);
            _totalObjects--;
        }

        public PickableBubble GetBubble()
        {
            // V�rifie si la limite de taille maximale est atteinte
            if (_totalObjects >= _maxPoolSize && _pool.CountInactive == 0)
            {
                Debug.LogWarning("Max pool size reached! Cannot spawn more bubbles.");
                return null; // Retourne null si la limite est atteinte
            }

            // Obtention d'une bulle du pool
            return _pool.Get();
        }
    }
}
