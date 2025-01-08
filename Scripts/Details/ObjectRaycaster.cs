using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRaycaster <T> where T : MonoBehaviour
{
    private const int MAX_OBJECTS_IN_MEMORY = 10;

    [Serializable]
    private class ObjectInMemory
    {
        private int _hashCode;
        private T _object;

        public int Key => _hashCode;
        public T Object => _object;

        public void Update(int hashCode, T obj)
        {
            _hashCode = hashCode;
            _object = obj;
        }
    }

    public event Action<T> OnSelected = delegate { };
    public event Action<T> OnDeselected = delegate { };

    private List<ObjectInMemory> _objectsInMemory = new(MAX_OBJECTS_IN_MEMORY);
    private int _listStep;

    private T _cacheObject;

    public ObjectRaycaster()
    {
        InitListObjectInMemory();
    }

    public void CheckObjectInRay(Vector3 origin, Vector3 direction, float maxDistance)
    {
        if (Physics.Raycast(origin, direction, out var hit, maxDistance))
        {
            T raycastObject = TryGetObjectInRay(hit);

            if (raycastObject != null)
            {
                if (_cacheObject != null)
                {
                    if (_cacheObject != raycastObject)
                    {
                        OnDeselected.Invoke(_cacheObject);
                    }
                }

                _cacheObject = raycastObject;
                IncListStep();

                OnSelected.Invoke(_cacheObject);

                return;
            }
        }

        if (_cacheObject != null)
        {
            OnDeselected.Invoke(_cacheObject);
        }

        _cacheObject = null;
    }

    private T TryGetObjectInRay(RaycastHit hit)
    {
        var objectInMemory = _objectsInMemory.Find(obj => obj.Key == hit.collider.gameObject.GetHashCode());

        if (objectInMemory != null)
        {
            return objectInMemory.Object;
        }

        if (hit.collider.TryGetComponent(out T raycastObject))
        {
            _objectsInMemory[_listStep].Update(hit.collider.gameObject.GetHashCode(), raycastObject);
            return raycastObject;
        }

        return null;
    }

    private void InitListObjectInMemory()
    {
        for (int i = 0; i < _objectsInMemory.Capacity; i++)
            _objectsInMemory.Add(new ObjectInMemory());
    }

    private void IncListStep()
    {
        if (_listStep < _objectsInMemory.Count - 1)
        {
            _listStep++;
        }
        else
        {
            _listStep = 0;
        }
    }
}