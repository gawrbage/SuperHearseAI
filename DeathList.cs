using System;
using ICities;
using ColossalFramework;
using UnityEngine;

namespace SuperHearseAI
{
    public class DeathList
    {
        internal class Node
        {
            public DeathClaim data;
            public Node next;
            internal Node()
            {

            }
            internal Node(DeathClaim claim)
            {
                data = claim;
            }
            internal Node(DeathClaim claim, Node node)
            {
                data = claim;
                next = node;
            }
        }

        private int count;
        private Node head;
        private Node tail;

        //public constructor
        public DeathList()
        {
            count = 0;
        }

        public void Add(DeathClaim claim)
        {
            count++;
            Node n = new Node(claim);

            if (head == null)
            {
                head = n;
                tail = n;
                return;
            }

            tail.next = n;
            tail = n;
            return;
        }

        public void FindAndDelete(DeathClaim claim)
        {
            if (count == 0) return; //Possibly could be removed for optimization
            if (claim == head.data) { DeleteHead(); return; }
            if (claim == tail.data) { DeleteTail(); return; }

            Node cursor = head;
            Node prevCursor = null;

            while (cursor.data != claim) { 
                prevCursor = cursor;
                cursor = cursor.next;
                if (cursor == null)
                {
                    return;
                }
            }
            prevCursor.next = null; //set the target to null
            cursor = cursor.next; //move the cursor forward
            prevCursor.next = cursor; //set the prevcursor next to the cursor

            count--;
        }

        public void DeleteAt(int index)
        {
            //IMPORTANT, these decrease the count by 1!!!
            if (index == 0) { DeleteHead(); return; }
            if (index == (count-1)) { DeleteTail(); return; }

            count--;

            Node cursor = head;

            Node prevCursor = null;

            for (int i = 0; i < index; i++)
            {
                prevCursor = cursor;
                cursor = cursor.next;
            }

            prevCursor.next = null; //set the target to null
            cursor = cursor.next; //move the cursor forward
            prevCursor.next = cursor; //set the prevcursor next to the cursor
        }

        private void DeleteHead()
        {
            count--; //reduce count by 1
            Node cursor = head.next; //set cursor to next head
            head = null; //delete head
            head = cursor; //set head to next
        }

        private void DeleteTail()
        {
            count--;//reduce count by 1
            Node cursor = head;//set cursor to head
            while (cursor.next != tail)//while the cursor isnt the tail, keep moving forward
            {
                cursor = cursor.next;
            }
            cursor.next = null; //set cursor next to null
            tail = cursor; //set cursor to the new tail
        }

        private DeathClaim[] GetArray()
        {
            DeathClaim[] arr = new DeathClaim[count];
            Node cursor = head;
            for (int i = 0; i < count; i++)
            {
                arr[i] = cursor.data;
                cursor = cursor.next;
            }
            return arr;
        }

        private void ClearPhantoms()
        {
            CitizenManager CM = Singleton<CitizenManager>.instance;

            DeathClaim[] claims = GetArray();

            count = 0;
            head = null;
            tail = null;

            for (int i = 0; i < claims.Length; i++)
            {
                switch (claims[i].location)
                {
                    case Citizen.Location.Home:
                        if(CM.m_citizens.m_buffer[claims[i].citizenID].m_homeBuilding != 0) Add(claims[i]);
                        break;
                    case Citizen.Location.Visit:
                        if (CM.m_citizens.m_buffer[claims[i].citizenID].m_visitBuilding != 0) Add(claims[i]);
                        break;
                    case Citizen.Location.Work:
                        if (CM.m_citizens.m_buffer[claims[i].citizenID].m_workBuilding != 0) Add(claims[i]);
                        break;
                    default:
                        DeathRegistry.RestClaim(claims[i]);
                        break;
                }
            }
        }

        //TODO: Implement FIFO method

        //TODO: rebuild this section, traverse the linked list instead of traversing the array
        public DeathClaim GetClosestAndPop(Vector3 pos)
        {
            DeathClaim claim;
            ClearPhantoms();
            switch (count) {
                case 0: return null;
                case 1: claim = head.data; DeleteHead(); return claim;
                default:
                    DeathClaim[] arr = GetArray();

                    claim = arr[0];
                    int claimIndex = 0;

                    //TODO: replace with manhatten distance formula
                    float currentDistance = Vector3.Distance(pos, claim.pos);
                    for (int i = 1; i < arr.Length; i++)
                    {
                        float nextDistance = Vector3.Distance(arr[i].pos, pos);
                        if (currentDistance > nextDistance)
                        {
                            claim = arr[i];
                            currentDistance = nextDistance;
                            claimIndex = i;
                        }

                    }

                    if (claimIndex == (count - 1)) {
                        DeleteTail();
                    } else {
                        DeleteAt(claimIndex);
                    }
                    
                    return claim;
            }
        }
    }
}