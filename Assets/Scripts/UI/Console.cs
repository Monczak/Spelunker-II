using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using DebugConsoleInterpreter;

public class Console : MonoBehaviour
{
    public static Console Instance;

    public TMP_Text consoleLog;
    public TMP_InputField inputField;

    public ScrollRect logScrollRect;

    public UIControls controls;

    private List<string> commandHistory;
    private int lastCommandIndex;

    public int commandHistoryLength = 100;
    public int maxLogLines = 100;

    int logLines = 0;

    private Animator animator;
    public bool consoleActive;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);

        inputField.onSubmit.AddListener((value) => OnSubmit());

        commandHistory = new List<string>();
        lastCommandIndex = 0;

        controls = new UIControls();

        controls.Keyboard.PreviousCommand.performed += ShowPreviousCommand;
        controls.Keyboard.NextCommand.performed += ShowNextCommand;
        controls.Keyboard.ShowHideConsole.performed += ToggleConsole;

        controls.Enable();

        animator = GetComponent<Animator>();
        consoleActive = false;
    }

    private void ToggleConsole(InputAction.CallbackContext obj)
    {
        if (obj.ReadValue<float>() == 1)
        {
            consoleActive = !consoleActive;
            animator.SetBool("Visible", consoleActive);

            if (consoleActive)
            {
                inputField.enabled = true;
                FocusInputField();
            }
            else
            {
                inputField.enabled = false;
            }
        }
        
    }

    private void ShowNextCommand(InputAction.CallbackContext obj)
    {
        if (obj.ReadValue<float>() == 1)
            if (inputField.isFocused && commandHistory.Count != 0 && consoleActive)
            {
                inputField.text = commandHistory[lastCommandIndex];
                lastCommandIndex += lastCommandIndex == commandHistory.Count - 1 ? 0 : 1;
            }
    }

    private void ShowPreviousCommand(InputAction.CallbackContext obj)
    {
        if (obj.ReadValue<float>() == 1)
            if (inputField.isFocused && commandHistory.Count != 0 && consoleActive)
            {
                inputField.text = commandHistory[lastCommandIndex];
                lastCommandIndex -= lastCommandIndex == 0 ? 0 : 1;
            }
    }

    private void OnSubmit()
    {
        if (inputField.isFocused && inputField.text != "" && consoleActive)
        {
            ExecuteCommand(inputField.text);
            commandHistory.Add(inputField.text);

            if (commandHistory.Count > commandHistoryLength)
                commandHistory.RemoveAt(0);

            lastCommandIndex = commandHistory.Count - 1;

            ClearInputField();
        }
    }

    private void Start()
    {
        ClearLog();
    }

    public void ClearLog()
    {
        consoleLog.text = "";
    }

    public void ClearInputField()
    {
        inputField.text = "";
    }

    public void Log(string message)
    {
        consoleLog.text += message + '\n';
        logLines += message.Count(c => c == '\n') + 1;

        if (logLines > maxLogLines)
        {
            int splitEndIndex = -1, newlinesToCut = 0;
            for (int i = 0; i < consoleLog.text.Length; i++)
            {
                if (consoleLog.text[i] == '\n')
                {
                    splitEndIndex = i;
                    if (++newlinesToCut == logLines - maxLogLines) break;
                }
                
            }

            logLines = maxLogLines;

            consoleLog.text = consoleLog.text.Substring(splitEndIndex + 1);
        }

        //StartCoroutine(ScrollLogToBottom());
        FocusInputField();
    }

    public bool ExecuteCommand(string command)
    {
        string commandResult;
        try
        {
            commandResult = CommandParser.Parse(command);
            Log(commandResult);
            return true;
        }
        catch (CommandNotFoundException)
        {
            Log($"Command {CommandParser.cmdlet} not found.");
        }
        catch (CommandExecutionException)
        {
            Log($"An exception occurred while executing this command.");
        }
        catch (System.Exception e)
        {
            Log($"Something went wrong: {e.Message}");
        }
        return false;

    }

    void FocusInputField()
    {
        inputField.Select();
        inputField.ActivateInputField();
    }

    IEnumerator ScrollLogToBottom()
    {
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();
        logScrollRect.verticalNormalizedPosition = 0;
        Canvas.ForceUpdateCanvases();
    }
}
