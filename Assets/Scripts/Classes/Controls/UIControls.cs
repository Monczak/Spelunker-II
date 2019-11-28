// GENERATED AUTOMATICALLY FROM 'Assets/Control Assets/UIControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @UIControls : IInputActionCollection, IDisposable
{
    private InputActionAsset asset;
    public @UIControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""UIControls"",
    ""maps"": [
        {
            ""name"": ""Mouse"",
            ""id"": ""16f828b1-f4f7-4ad2-a16d-960844a50cb8"",
            ""actions"": [
                {
                    ""name"": ""Left Click"",
                    ""type"": ""PassThrough"",
                    ""id"": ""6604b07c-7071-48f6-b4d1-39fff921b89e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Right Click"",
                    ""type"": ""PassThrough"",
                    ""id"": ""7007cc5f-21d1-49d5-9236-9af1f87de50c"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Scroll"",
                    ""type"": ""PassThrough"",
                    ""id"": ""650b70f6-568b-45d8-9765-04ed0fcf7e9f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Point"",
                    ""type"": ""PassThrough"",
                    ""id"": ""4d476f35-89a3-47ff-90b9-d18deb8a897f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""4a8c3e96-8311-47ab-b66c-cbc46dd465ef"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b63d246d-cacf-4dc4-9048-b57ea7da94bb"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""114b12b1-b1b3-4e36-8434-01f714d630a9"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2fcd660d-c7e6-4649-a2a3-3b8d912b1c85"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard"",
            ""id"": ""8cf10ed8-5b0b-4977-8f64-38e775c21d2b"",
            ""actions"": [
                {
                    ""name"": ""Submit"",
                    ""type"": ""PassThrough"",
                    ""id"": ""540d1b2c-9b05-45ef-b556-d28a1eaed05e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""b8fd123f-d764-4ccb-8825-0ad0015ebe1e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Previous Command"",
                    ""type"": ""PassThrough"",
                    ""id"": ""9faca358-938b-44d7-9692-e27cb50349ea"",
                    ""expectedControlType"": ""Key"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Next Command"",
                    ""type"": ""PassThrough"",
                    ""id"": ""01ec466c-ba1c-4008-97a5-b5a9fc46f814"",
                    ""expectedControlType"": ""Key"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Show/Hide Console"",
                    ""type"": ""Button"",
                    ""id"": ""2bfa3c88-1d37-4ef5-a6e1-a189207c6ffb"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b1d843ad-8e99-4fb4-b5a6-584ce1a04f58"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3deb8d01-297f-40cf-9716-1a2f01a5e653"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c1e140bb-2ea8-49d6-a16e-544522db8873"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Previous Command"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""758062fd-1152-48f3-95a1-42378b2bf3df"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Next Command"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9b0796f3-9747-4793-b085-4a4e417f5178"",
                    ""path"": ""<Keyboard>/backslash"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Show/Hide Console"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Mouse
        m_Mouse = asset.FindActionMap("Mouse", throwIfNotFound: true);
        m_Mouse_LeftClick = m_Mouse.FindAction("Left Click", throwIfNotFound: true);
        m_Mouse_RightClick = m_Mouse.FindAction("Right Click", throwIfNotFound: true);
        m_Mouse_Scroll = m_Mouse.FindAction("Scroll", throwIfNotFound: true);
        m_Mouse_Point = m_Mouse.FindAction("Point", throwIfNotFound: true);
        // Keyboard
        m_Keyboard = asset.FindActionMap("Keyboard", throwIfNotFound: true);
        m_Keyboard_Submit = m_Keyboard.FindAction("Submit", throwIfNotFound: true);
        m_Keyboard_Cancel = m_Keyboard.FindAction("Cancel", throwIfNotFound: true);
        m_Keyboard_PreviousCommand = m_Keyboard.FindAction("Previous Command", throwIfNotFound: true);
        m_Keyboard_NextCommand = m_Keyboard.FindAction("Next Command", throwIfNotFound: true);
        m_Keyboard_ShowHideConsole = m_Keyboard.FindAction("Show/Hide Console", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Mouse
    private readonly InputActionMap m_Mouse;
    private IMouseActions m_MouseActionsCallbackInterface;
    private readonly InputAction m_Mouse_LeftClick;
    private readonly InputAction m_Mouse_RightClick;
    private readonly InputAction m_Mouse_Scroll;
    private readonly InputAction m_Mouse_Point;
    public struct MouseActions
    {
        private @UIControls m_Wrapper;
        public MouseActions(@UIControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftClick => m_Wrapper.m_Mouse_LeftClick;
        public InputAction @RightClick => m_Wrapper.m_Mouse_RightClick;
        public InputAction @Scroll => m_Wrapper.m_Mouse_Scroll;
        public InputAction @Point => m_Wrapper.m_Mouse_Point;
        public InputActionMap Get() { return m_Wrapper.m_Mouse; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MouseActions set) { return set.Get(); }
        public void SetCallbacks(IMouseActions instance)
        {
            if (m_Wrapper.m_MouseActionsCallbackInterface != null)
            {
                @LeftClick.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnLeftClick;
                @LeftClick.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnLeftClick;
                @LeftClick.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnLeftClick;
                @RightClick.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnRightClick;
                @RightClick.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnRightClick;
                @RightClick.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnRightClick;
                @Scroll.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnScroll;
                @Scroll.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnScroll;
                @Scroll.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnScroll;
                @Point.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnPoint;
                @Point.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnPoint;
                @Point.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnPoint;
            }
            m_Wrapper.m_MouseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @LeftClick.started += instance.OnLeftClick;
                @LeftClick.performed += instance.OnLeftClick;
                @LeftClick.canceled += instance.OnLeftClick;
                @RightClick.started += instance.OnRightClick;
                @RightClick.performed += instance.OnRightClick;
                @RightClick.canceled += instance.OnRightClick;
                @Scroll.started += instance.OnScroll;
                @Scroll.performed += instance.OnScroll;
                @Scroll.canceled += instance.OnScroll;
                @Point.started += instance.OnPoint;
                @Point.performed += instance.OnPoint;
                @Point.canceled += instance.OnPoint;
            }
        }
    }
    public MouseActions @Mouse => new MouseActions(this);

    // Keyboard
    private readonly InputActionMap m_Keyboard;
    private IKeyboardActions m_KeyboardActionsCallbackInterface;
    private readonly InputAction m_Keyboard_Submit;
    private readonly InputAction m_Keyboard_Cancel;
    private readonly InputAction m_Keyboard_PreviousCommand;
    private readonly InputAction m_Keyboard_NextCommand;
    private readonly InputAction m_Keyboard_ShowHideConsole;
    public struct KeyboardActions
    {
        private @UIControls m_Wrapper;
        public KeyboardActions(@UIControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Submit => m_Wrapper.m_Keyboard_Submit;
        public InputAction @Cancel => m_Wrapper.m_Keyboard_Cancel;
        public InputAction @PreviousCommand => m_Wrapper.m_Keyboard_PreviousCommand;
        public InputAction @NextCommand => m_Wrapper.m_Keyboard_NextCommand;
        public InputAction @ShowHideConsole => m_Wrapper.m_Keyboard_ShowHideConsole;
        public InputActionMap Get() { return m_Wrapper.m_Keyboard; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(KeyboardActions set) { return set.Get(); }
        public void SetCallbacks(IKeyboardActions instance)
        {
            if (m_Wrapper.m_KeyboardActionsCallbackInterface != null)
            {
                @Submit.started -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnSubmit;
                @Submit.performed -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnSubmit;
                @Submit.canceled -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnSubmit;
                @Cancel.started -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnCancel;
                @Cancel.performed -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnCancel;
                @Cancel.canceled -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnCancel;
                @PreviousCommand.started -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnPreviousCommand;
                @PreviousCommand.performed -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnPreviousCommand;
                @PreviousCommand.canceled -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnPreviousCommand;
                @NextCommand.started -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnNextCommand;
                @NextCommand.performed -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnNextCommand;
                @NextCommand.canceled -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnNextCommand;
                @ShowHideConsole.started -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnShowHideConsole;
                @ShowHideConsole.performed -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnShowHideConsole;
                @ShowHideConsole.canceled -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnShowHideConsole;
            }
            m_Wrapper.m_KeyboardActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Submit.started += instance.OnSubmit;
                @Submit.performed += instance.OnSubmit;
                @Submit.canceled += instance.OnSubmit;
                @Cancel.started += instance.OnCancel;
                @Cancel.performed += instance.OnCancel;
                @Cancel.canceled += instance.OnCancel;
                @PreviousCommand.started += instance.OnPreviousCommand;
                @PreviousCommand.performed += instance.OnPreviousCommand;
                @PreviousCommand.canceled += instance.OnPreviousCommand;
                @NextCommand.started += instance.OnNextCommand;
                @NextCommand.performed += instance.OnNextCommand;
                @NextCommand.canceled += instance.OnNextCommand;
                @ShowHideConsole.started += instance.OnShowHideConsole;
                @ShowHideConsole.performed += instance.OnShowHideConsole;
                @ShowHideConsole.canceled += instance.OnShowHideConsole;
            }
        }
    }
    public KeyboardActions @Keyboard => new KeyboardActions(this);
    public interface IMouseActions
    {
        void OnLeftClick(InputAction.CallbackContext context);
        void OnRightClick(InputAction.CallbackContext context);
        void OnScroll(InputAction.CallbackContext context);
        void OnPoint(InputAction.CallbackContext context);
    }
    public interface IKeyboardActions
    {
        void OnSubmit(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
        void OnPreviousCommand(InputAction.CallbackContext context);
        void OnNextCommand(InputAction.CallbackContext context);
        void OnShowHideConsole(InputAction.CallbackContext context);
    }
}
