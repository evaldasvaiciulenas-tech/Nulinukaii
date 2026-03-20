using UnityEngine;

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
    public float sendInterval = 0.1f;

    public AudioClip sendStudentSound;
    public AudioClip receiveStudentSound;
    private AudioSource audioSource;
    public AudioClip captureSound;

    private float sendTimer = 0f;
    private int studentsToSend = 0;
    private NodePastatas sendTarget;
    private bool isSending = false;

    public GameObject studentPrefab;
    public LineRenderer dragLine;

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
                dynamicInterval = 0.3f;
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

        if (selectedNode == this && Input.GetMouseButton(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            dragLine.positionCount = 2;
            dragLine.SetPosition(0, transform.position);
            dragLine.SetPosition(1, mouseWorld);
        }

        if (selectedNode == this && Input.GetMouseButtonUp(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(
                Camera.main.ScreenToWorldPoint(Input.mousePosition),
                Vector2.zero
            );

            if (hit.collider != null)
            {
                NodePastatas target = hit.collider.GetComponent<NodePastatas>();
                if (target != null && target != this)
                    SendStudents(target);
            }

            selectedNode = null;
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
        selectedNode = this;
    }

    public void SendStudents(NodePastatas target)
    {
        if (studentPrefab == null) return;

        int sendAmount = studentCount / 2;
        if (sendAmount <= 0) return;

        studentCount -= sendAmount;
        UpdateText();

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
            dragLine.positionCount = 0;
    }
}