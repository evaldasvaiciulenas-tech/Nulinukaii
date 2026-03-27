using UnityEngine;
using System.Collections.Generic;

public class NodePastatas : MonoBehaviour
{
    public enum OwnerType { Neutral, Player, AI }
    public OwnerType owner = OwnerType.Neutral;

    public Sprite neutralSprite;
    public Sprite playerSprite;
    public Sprite aiSprite;

    public int studentCount = 10;
    public int maxStudents = 100;

    public float generateInterval = 1f;
    public int generateAmount = 1;

    public int movingStudents = 0;

    private NodePastatas currentTarget;
    private OwnerType sendingOwner;

    public float sendInterval = 0.1f;
    public int minStudentsToSend = 5;

    private Dictionary<NodePastatas, float> targetCooldowns = new Dictionary<NodePastatas, float>();
    public float connectionCooldown = 1f;

    public AudioClip sendStudentSound;
    public AudioClip receiveStudentSound;
    public AudioClip captureSound;

    private AudioSource audioSource;

    private float sendTimer = 0f;
    private int studentsToSend = 0;
    private NodePastatas sendTarget;
    private bool isSending = false;

    public GameObject studentPrefab;
    public LineRenderer dragLine;

    public static int playerActiveLines = 0;
    public static int aiActiveLines = 0;
    public int maxActiveLines = 3;

    private float timer;
    private TextMesh countText;

    private static NodePastatas selectedNode;

    void Start()
    {
        countText = GetComponentInChildren<TextMesh>();
        UpdateText();

        dragLine.positionCount = 0;

        UpdateSprite();

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if ((owner == OwnerType.Player || owner == OwnerType.AI) && studentCount < maxStudents)
        {
            float dynamicInterval = generateInterval;

            if (studentCount > 50)
                dynamicInterval = 0.45f;
            else if (studentCount > 20)
                dynamicInterval = 0.5f;

            timer += Time.deltaTime;

            if (timer >= dynamicInterval)
            {
                studentCount += generateAmount;
                if (studentCount > maxStudents) studentCount = maxStudents;

                timer = 0f;
                UpdateText();
            }
        }

        if (selectedNode == this && owner == OwnerType.Player && Input.GetMouseButton(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            if (dragLine.positionCount < 2)
                dragLine.positionCount = 2;

            dragLine.SetPosition(0, transform.position);
            dragLine.SetPosition(1, mouseWorld);
        }

        if (selectedNode == this && owner == OwnerType.Player && Input.GetMouseButtonUp(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            bool success = false;

            if (hit != null)
            {
                NodePastatas target = hit.GetComponent<NodePastatas>();

                if (target != null && target != this)
                {
                    success = SendStudents(target);
                }
            }

            if (success)
            {
                selectedNode = null;
            }
        }

        if (isSending && studentsToSend > 0)
        {
            sendTimer += Time.deltaTime;

            if (sendTimer >= sendInterval)
            {
                sendTimer = 0f;

                Vector3 offset = Random.insideUnitCircle * 0.2f;

                GameObject s = Instantiate(studentPrefab, transform.position + offset, Quaternion.identity);
                s.GetComponent<Student>().SetTarget(sendTarget, this);

                if (audioSource != null && sendStudentSound != null)
                    audioSource.PlayOneShot(sendStudentSound);

                studentsToSend--;
            }
        }
    }

    void UpdateSprite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        if (owner == OwnerType.Neutral && neutralSprite != null)
            sr.sprite = neutralSprite;
        else if (owner == OwnerType.Player && playerSprite != null)
            sr.sprite = playerSprite;
        else if (owner == OwnerType.AI && aiSprite != null)
            sr.sprite = aiSprite;
    }

    void OnMouseDown()
    {
        if (owner != OwnerType.Player)
            return;

        selectedNode = this;

        dragLine.positionCount = 2;
        dragLine.SetPosition(0, transform.position);
        dragLine.SetPosition(1, transform.position);
    }

    public bool SendStudents(NodePastatas target)
    {
        sendingOwner = owner;

        if (owner == OwnerType.Player && playerActiveLines >= maxActiveLines)
            return false;

        if (owner == OwnerType.AI && aiActiveLines >= maxActiveLines)
            return false;

        if (targetCooldowns.ContainsKey(target))
        {
            if (Time.time < targetCooldowns[target] + connectionCooldown)
                return false;
        }

        if (studentCount < minStudentsToSend)
            return false;

        if (studentPrefab == null)
            return false;

        int sendAmount = Mathf.Clamp(studentCount / 3, 1, 20);

        if (sendAmount <= 0)
            return false;

        studentCount -= sendAmount;
        UpdateText();

        targetCooldowns[target] = Time.time;

        if (owner == OwnerType.Player)
            playerActiveLines++;
        else if (owner == OwnerType.AI)
            aiActiveLines++;

        sendTarget = target;
        studentsToSend = sendAmount;
        isSending = true;

        currentTarget = target;
        movingStudents = sendAmount;

        dragLine.positionCount = 2;
        dragLine.SetPosition(0, transform.position);
        dragLine.SetPosition(1, target.transform.position);

        if (audioSource != null && sendStudentSound != null)
            audioSource.PlayOneShot(sendStudentSound);

        return true;
    }

    public void ReceiveStudent(NodePastatas source)
    {
        if (owner == source.owner)
        {
            if (studentCount < maxStudents)
            {
                studentCount++;
                UpdateText();
            }
            return;
        }

        studentCount--;

        if (studentCount <= 0)
        {
            owner = source.owner;
            studentCount = 1;

            UpdateSprite();
            UpdateText();

            if (audioSource != null && captureSound != null)
                audioSource.PlayOneShot(captureSound);
        }
        else
        {
            UpdateText();
        }
    }

    void UpdateText()
    {
        if (countText != null)
            countText.text = studentCount.ToString();
    }

    public void StudentArrived()
    {
        movingStudents--;

        if (movingStudents <= 0)
        {
            dragLine.positionCount = 0;
            isSending = false;

            if (sendingOwner == OwnerType.Player)
                playerActiveLines--;
            else if (sendingOwner == OwnerType.AI)
                aiActiveLines--;
        }
    }
}