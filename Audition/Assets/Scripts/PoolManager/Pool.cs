/**
 *    Copyright (C) 2017 tongtunggiang.com
 *
 *    This program is free software: you can redistribute it and/or  modify
 *    it under the terms of the GNU Affero General Public License, version 3,
 *    as published by the Free Software Foundation.
 *
 *    This program is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 *    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *    GNU Affero General Public License for more details.
 *
 *    You should have received a copy of the GNU Affero General Public License
 *    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;

    [SerializeField]
    int initialPoolsize = 10;

    public bool ShouldExpand = true;
    private int orginalPoolsize;
    public bool ShouldRemove = true;
    public GameObject parentAnchor;

    Stack<GameObject> pooledInstances;
    List<GameObject> aliveInstances;

    public GameObject Prefab { get { return prefab; } }

    void Awake()
    {
        pooledInstances = new Stack<GameObject>();
        for (int i = 0; i < initialPoolsize; i++)
        {
            GameObject instance = Instantiate(prefab);
            instance.transform.SetParent(transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = Vector3.one;
            instance.transform.localEulerAngles = Vector3.zero;
            instance.SetActive(false);

            pooledInstances.Push(instance);
        }

        aliveInstances = new List<GameObject>();

        orginalPoolsize = initialPoolsize;
    }

    /// <summary>
    /// Bring a new game object to life by taking a dead one from
    /// the waiting pool. If all objects in the pool are alive, 
    /// create a new one and add it to the pool.
    /// </summary>
    public GameObject Spawn(Vector3 position, 
        Quaternion rotation, 
        Vector3 scale, 
        Transform parent = null,
        bool useLocalPosition = false,
        bool useLocalRotation = false)
    {
        //Debug.Log("Spawn: pooledInstances.Count = " + pooledInstances.Count);
        //Debug.Log("Spawn: aliveInstances.Count = " + pooledInstances.Count);
        //GameObject customParent = GameObject.Find("PickUpSpawn");
        //if(customParent != null)
        //{
            //Debug.Log("Found PickUpSpawn");
        //}
        if (pooledInstances.Count <= 0) // Every game object has been spawned!
        {
            if(ShouldExpand == false)
            {
                return null;
            }

            GameObject newlyInstantiatedObject = Instantiate(prefab);

            //newlyInstantiatedObject.transform.SetParent(parent);
            newlyInstantiatedObject.transform.SetParent(parentAnchor.transform);
            //newlyInstantiatedObject.transform.SetParent(GameObject.Find("MoveBar").transform);
            

            if (useLocalPosition)
                newlyInstantiatedObject.transform.localPosition = position;
            else
                newlyInstantiatedObject.transform.position = position;

            if (useLocalRotation)
                newlyInstantiatedObject.transform.localRotation = rotation;
            else
                newlyInstantiatedObject.transform.rotation = rotation;

            newlyInstantiatedObject.transform.localScale = scale;

            aliveInstances.Add(newlyInstantiatedObject);
            return newlyInstantiatedObject;
        }

        GameObject obj = pooledInstances.Pop();

        obj.transform.SetParent(parentAnchor.transform);
        //obj.transform.SetParent(GameObject.Find("MoveBar").transform);

        if (useLocalPosition)
            obj.transform.localPosition = position;
        else
            obj.transform.position = position;

        if (useLocalRotation)
            obj.transform.localRotation = rotation;
        else
            obj.transform.rotation = rotation;
        obj.transform.localScale = scale;

        obj.SetActive(true);

        aliveInstances.Add(obj);

        return obj;
    }

    /// <summary>
    /// Deactivate an object and add it back to the pool, given that it's
    /// in alive objects array.
    /// </summary>
    /// <param name="obj"></param>
    public void Kill(GameObject obj)
    {
        int index = aliveInstances.FindIndex(o => obj == o);
        if (index == -1 || ShouldRemove == true)
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);

        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.transform.localEulerAngles = Vector3.zero;

        aliveInstances.RemoveAt(index);
        pooledInstances.Push(obj);
    }

    public void CheckUpdate(GameObject obj)
    {
        //foreach(GameObject obj in aliveInstances)
        //{
            //Debug.Log("Obj name: " + obj.tag);
            //if(obj.CompareTag("PickupExit"))
            //{
                //int index = aliveInstances.FindIndex(o => obj == o);
                //Debug.Log("Found die at index " + index);
            //}
        //}
    }

    public bool IsResponsibleForObject(GameObject obj)
    {
        int index = aliveInstances.FindIndex(o => ReferenceEquals(obj, o));
        if (index == -1)
        {
            return false;
        }

        return true;
    }
}
