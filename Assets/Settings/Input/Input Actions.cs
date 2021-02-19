// GENERATED AUTOMATICALLY FROM 'Assets/Settings/Input/Input Actions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Input Actions"",
    ""maps"": [
        {
            ""name"": ""PC Player"",
            ""id"": ""9e67ea2c-d582-42c5-bfd4-c9e44f9858cc"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""8350f684-c428-4ce3-84bc-ce246c07c550"",
                    ""expectedControlType"": ""Analog"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""1f6e6946-99a3-48ca-a309-0f2cafa6374f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Grab"",
                    ""type"": ""Button"",
                    ""id"": ""93509acc-73b2-4436-9b60-702b3bdb79e5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""2996c7d4-380c-494b-8324-8349af11127d"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""63339003-2752-4cb5-ac7e-58d4f3cb2ddf"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""10077820-0464-479b-b086-bd33e72abc10"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""1a123ab0-9cf9-43bf-b5d5-222592ce5dd8"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""e13b1eb3-2477-4fbd-bc70-7f7e6378cf72"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrow Keys"",
                    ""id"": ""dc4f3220-717d-41f3-a036-ba35ccbd8b37"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1f051370-e41f-4d38-82ae-62c1fe71b37a"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""6dc23747-2028-432a-aadc-bc927a9fdc1c"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""9d333cb8-c54a-4aa8-bb6d-ee460d00ff51"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""0d83121b-e73f-4c9c-b8c4-e007828e0d91"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""L Stick"",
                    ""id"": ""0c8241ea-9988-42d9-a92f-83d9e9f3c9a8"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ffa2c98b-533c-4588-8494-042c75b46cf7"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""55dc50a6-ae77-42eb-9f6c-2c959d7494fc"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0e610abd-ae2e-4094-9920-34bd6f0bfbc7"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""41f29a9b-bb70-49b1-84a4-1e70f70cd5bf"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""fbe6527a-a1dd-4dba-8199-88dc898af4aa"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dabd2bf1-4165-4758-aad1-a05f296d12d7"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""082b241b-4533-4dcd-86e0-0f8dad5853b1"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Grab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""141a8599-3ed7-467f-a30f-6ed6627b3d08"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Grab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5dbac1bc-f3d5-43df-9ebe-a8d8c1b13bd0"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Grab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Mobile Player"",
            ""id"": ""eaf2e0a7-d439-40c4-b3ae-46b5210e4f2a"",
            ""actions"": [
                {
                    ""name"": ""Tap Start"",
                    ""type"": ""Value"",
                    ""id"": ""9673526b-2628-4f9d-9cc7-d4bf9393cbee"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Tap End"",
                    ""type"": ""Value"",
                    ""id"": ""39e790fe-2c7a-4572-9483-da1c8b5e8f04"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""14cdf4fd-6fb8-45ec-9bb4-b4c71bf70bd1"",
                    ""path"": ""<Touchscreen>/primaryTouch/startPosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touchscreen"",
                    ""action"": ""Tap Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0c9afe4c-290c-4912-97a0-55a87245f79b"",
                    ""path"": ""<Touchscreen>/primaryTouch/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touchscreen"",
                    ""action"": ""Tap End"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Touchscreen"",
            ""bindingGroup"": ""Touchscreen"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // PC Player
        m_PCPlayer = asset.FindActionMap("PC Player", throwIfNotFound: true);
        m_PCPlayer_Movement = m_PCPlayer.FindAction("Movement", throwIfNotFound: true);
        m_PCPlayer_Jump = m_PCPlayer.FindAction("Jump", throwIfNotFound: true);
        m_PCPlayer_Grab = m_PCPlayer.FindAction("Grab", throwIfNotFound: true);
        // Mobile Player
        m_MobilePlayer = asset.FindActionMap("Mobile Player", throwIfNotFound: true);
        m_MobilePlayer_TapStart = m_MobilePlayer.FindAction("Tap Start", throwIfNotFound: true);
        m_MobilePlayer_TapEnd = m_MobilePlayer.FindAction("Tap End", throwIfNotFound: true);
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

    // PC Player
    private readonly InputActionMap m_PCPlayer;
    private IPCPlayerActions m_PCPlayerActionsCallbackInterface;
    private readonly InputAction m_PCPlayer_Movement;
    private readonly InputAction m_PCPlayer_Jump;
    private readonly InputAction m_PCPlayer_Grab;
    public struct PCPlayerActions
    {
        private @InputActions m_Wrapper;
        public PCPlayerActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_PCPlayer_Movement;
        public InputAction @Jump => m_Wrapper.m_PCPlayer_Jump;
        public InputAction @Grab => m_Wrapper.m_PCPlayer_Grab;
        public InputActionMap Get() { return m_Wrapper.m_PCPlayer; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PCPlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPCPlayerActions instance)
        {
            if (m_Wrapper.m_PCPlayerActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PCPlayerActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PCPlayerActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PCPlayerActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_PCPlayerActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PCPlayerActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PCPlayerActionsCallbackInterface.OnJump;
                @Grab.started -= m_Wrapper.m_PCPlayerActionsCallbackInterface.OnGrab;
                @Grab.performed -= m_Wrapper.m_PCPlayerActionsCallbackInterface.OnGrab;
                @Grab.canceled -= m_Wrapper.m_PCPlayerActionsCallbackInterface.OnGrab;
            }
            m_Wrapper.m_PCPlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Grab.started += instance.OnGrab;
                @Grab.performed += instance.OnGrab;
                @Grab.canceled += instance.OnGrab;
            }
        }
    }
    public PCPlayerActions @PCPlayer => new PCPlayerActions(this);

    // Mobile Player
    private readonly InputActionMap m_MobilePlayer;
    private IMobilePlayerActions m_MobilePlayerActionsCallbackInterface;
    private readonly InputAction m_MobilePlayer_TapStart;
    private readonly InputAction m_MobilePlayer_TapEnd;
    public struct MobilePlayerActions
    {
        private @InputActions m_Wrapper;
        public MobilePlayerActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @TapStart => m_Wrapper.m_MobilePlayer_TapStart;
        public InputAction @TapEnd => m_Wrapper.m_MobilePlayer_TapEnd;
        public InputActionMap Get() { return m_Wrapper.m_MobilePlayer; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MobilePlayerActions set) { return set.Get(); }
        public void SetCallbacks(IMobilePlayerActions instance)
        {
            if (m_Wrapper.m_MobilePlayerActionsCallbackInterface != null)
            {
                @TapStart.started -= m_Wrapper.m_MobilePlayerActionsCallbackInterface.OnTapStart;
                @TapStart.performed -= m_Wrapper.m_MobilePlayerActionsCallbackInterface.OnTapStart;
                @TapStart.canceled -= m_Wrapper.m_MobilePlayerActionsCallbackInterface.OnTapStart;
                @TapEnd.started -= m_Wrapper.m_MobilePlayerActionsCallbackInterface.OnTapEnd;
                @TapEnd.performed -= m_Wrapper.m_MobilePlayerActionsCallbackInterface.OnTapEnd;
                @TapEnd.canceled -= m_Wrapper.m_MobilePlayerActionsCallbackInterface.OnTapEnd;
            }
            m_Wrapper.m_MobilePlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @TapStart.started += instance.OnTapStart;
                @TapStart.performed += instance.OnTapStart;
                @TapStart.canceled += instance.OnTapStart;
                @TapEnd.started += instance.OnTapEnd;
                @TapEnd.performed += instance.OnTapEnd;
                @TapEnd.canceled += instance.OnTapEnd;
            }
        }
    }
    public MobilePlayerActions @MobilePlayer => new MobilePlayerActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_TouchscreenSchemeIndex = -1;
    public InputControlScheme TouchscreenScheme
    {
        get
        {
            if (m_TouchscreenSchemeIndex == -1) m_TouchscreenSchemeIndex = asset.FindControlSchemeIndex("Touchscreen");
            return asset.controlSchemes[m_TouchscreenSchemeIndex];
        }
    }
    public interface IPCPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnGrab(InputAction.CallbackContext context);
    }
    public interface IMobilePlayerActions
    {
        void OnTapStart(InputAction.CallbackContext context);
        void OnTapEnd(InputAction.CallbackContext context);
    }
}
