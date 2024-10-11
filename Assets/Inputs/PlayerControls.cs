//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Inputs/PlayerControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Crawl"",
            ""id"": ""2df64cf5-14bb-4ca8-8fb4-e989aa893e1e"",
            ""actions"": [
                {
                    ""name"": ""LeftArm"",
                    ""type"": ""Button"",
                    ""id"": ""cbc558c8-c6c6-4a4a-85c1-13d1306ba50c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightArm"",
                    ""type"": ""Button"",
                    ""id"": ""21896bff-87e7-4133-be7c-26b6b63931a6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CamLeft"",
                    ""type"": ""Button"",
                    ""id"": ""8ce0d482-0f87-474f-b95f-cb71ad83d6b4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CamRight"",
                    ""type"": ""Button"",
                    ""id"": ""901831c1-30f1-453e-b91c-15e629aaf31b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CamBehindLeft"",
                    ""type"": ""Button"",
                    ""id"": ""c5c9b32f-83c1-47b9-91b5-7ded97a57f32"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CamBehindRight"",
                    ""type"": ""Button"",
                    ""id"": ""160e563b-2d59-4778-a5fd-2e9b8028fe39"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Punch"",
                    ""type"": ""Button"",
                    ""id"": ""1e69b5d3-fe38-423b-8a46-556cfc91ca6a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Kick"",
                    ""type"": ""Button"",
                    ""id"": ""f9f2f0b8-77dc-427a-a645-c6a295bba383"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CheckMap"",
                    ""type"": ""Button"",
                    ""id"": ""f8879268-67fa-4419-b876-67857422b7ce"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7be1254e-04c7-4c35-977a-abcab8112fe3"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftArm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""09f5f53e-66fd-49c0-82fe-d49df49b3775"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightArm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5f891f6d-34e0-488a-9e18-c4b2d1dc098e"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CamLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e36722d0-9910-4b9e-8292-54df1f845a4f"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CamRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee2e68f7-73b2-4836-a50b-90173e4b06d5"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CamBehindLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1cc1f28f-986e-418c-989c-7c4de8fabe63"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CamBehindRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ba90f12d-48e1-43ec-952b-b72b5944b1c1"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Punch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4d4849b3-3827-44d9-9a93-b92bedd1b931"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Kick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8b3af883-e49f-4823-959d-c247aa493758"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CheckMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Crawl
        m_Crawl = asset.FindActionMap("Crawl", throwIfNotFound: true);
        m_Crawl_LeftArm = m_Crawl.FindAction("LeftArm", throwIfNotFound: true);
        m_Crawl_RightArm = m_Crawl.FindAction("RightArm", throwIfNotFound: true);
        m_Crawl_CamLeft = m_Crawl.FindAction("CamLeft", throwIfNotFound: true);
        m_Crawl_CamRight = m_Crawl.FindAction("CamRight", throwIfNotFound: true);
        m_Crawl_CamBehindLeft = m_Crawl.FindAction("CamBehindLeft", throwIfNotFound: true);
        m_Crawl_CamBehindRight = m_Crawl.FindAction("CamBehindRight", throwIfNotFound: true);
        m_Crawl_Punch = m_Crawl.FindAction("Punch", throwIfNotFound: true);
        m_Crawl_Kick = m_Crawl.FindAction("Kick", throwIfNotFound: true);
        m_Crawl_CheckMap = m_Crawl.FindAction("CheckMap", throwIfNotFound: true);
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Crawl
    private readonly InputActionMap m_Crawl;
    private List<ICrawlActions> m_CrawlActionsCallbackInterfaces = new List<ICrawlActions>();
    private readonly InputAction m_Crawl_LeftArm;
    private readonly InputAction m_Crawl_RightArm;
    private readonly InputAction m_Crawl_CamLeft;
    private readonly InputAction m_Crawl_CamRight;
    private readonly InputAction m_Crawl_CamBehindLeft;
    private readonly InputAction m_Crawl_CamBehindRight;
    private readonly InputAction m_Crawl_Punch;
    private readonly InputAction m_Crawl_Kick;
    private readonly InputAction m_Crawl_CheckMap;
    public struct CrawlActions
    {
        private @PlayerControls m_Wrapper;
        public CrawlActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftArm => m_Wrapper.m_Crawl_LeftArm;
        public InputAction @RightArm => m_Wrapper.m_Crawl_RightArm;
        public InputAction @CamLeft => m_Wrapper.m_Crawl_CamLeft;
        public InputAction @CamRight => m_Wrapper.m_Crawl_CamRight;
        public InputAction @CamBehindLeft => m_Wrapper.m_Crawl_CamBehindLeft;
        public InputAction @CamBehindRight => m_Wrapper.m_Crawl_CamBehindRight;
        public InputAction @Punch => m_Wrapper.m_Crawl_Punch;
        public InputAction @Kick => m_Wrapper.m_Crawl_Kick;
        public InputAction @CheckMap => m_Wrapper.m_Crawl_CheckMap;
        public InputActionMap Get() { return m_Wrapper.m_Crawl; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CrawlActions set) { return set.Get(); }
        public void AddCallbacks(ICrawlActions instance)
        {
            if (instance == null || m_Wrapper.m_CrawlActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_CrawlActionsCallbackInterfaces.Add(instance);
            @LeftArm.started += instance.OnLeftArm;
            @LeftArm.performed += instance.OnLeftArm;
            @LeftArm.canceled += instance.OnLeftArm;
            @RightArm.started += instance.OnRightArm;
            @RightArm.performed += instance.OnRightArm;
            @RightArm.canceled += instance.OnRightArm;
            @CamLeft.started += instance.OnCamLeft;
            @CamLeft.performed += instance.OnCamLeft;
            @CamLeft.canceled += instance.OnCamLeft;
            @CamRight.started += instance.OnCamRight;
            @CamRight.performed += instance.OnCamRight;
            @CamRight.canceled += instance.OnCamRight;
            @CamBehindLeft.started += instance.OnCamBehindLeft;
            @CamBehindLeft.performed += instance.OnCamBehindLeft;
            @CamBehindLeft.canceled += instance.OnCamBehindLeft;
            @CamBehindRight.started += instance.OnCamBehindRight;
            @CamBehindRight.performed += instance.OnCamBehindRight;
            @CamBehindRight.canceled += instance.OnCamBehindRight;
            @Punch.started += instance.OnPunch;
            @Punch.performed += instance.OnPunch;
            @Punch.canceled += instance.OnPunch;
            @Kick.started += instance.OnKick;
            @Kick.performed += instance.OnKick;
            @Kick.canceled += instance.OnKick;
            @CheckMap.started += instance.OnCheckMap;
            @CheckMap.performed += instance.OnCheckMap;
            @CheckMap.canceled += instance.OnCheckMap;
        }

        private void UnregisterCallbacks(ICrawlActions instance)
        {
            @LeftArm.started -= instance.OnLeftArm;
            @LeftArm.performed -= instance.OnLeftArm;
            @LeftArm.canceled -= instance.OnLeftArm;
            @RightArm.started -= instance.OnRightArm;
            @RightArm.performed -= instance.OnRightArm;
            @RightArm.canceled -= instance.OnRightArm;
            @CamLeft.started -= instance.OnCamLeft;
            @CamLeft.performed -= instance.OnCamLeft;
            @CamLeft.canceled -= instance.OnCamLeft;
            @CamRight.started -= instance.OnCamRight;
            @CamRight.performed -= instance.OnCamRight;
            @CamRight.canceled -= instance.OnCamRight;
            @CamBehindLeft.started -= instance.OnCamBehindLeft;
            @CamBehindLeft.performed -= instance.OnCamBehindLeft;
            @CamBehindLeft.canceled -= instance.OnCamBehindLeft;
            @CamBehindRight.started -= instance.OnCamBehindRight;
            @CamBehindRight.performed -= instance.OnCamBehindRight;
            @CamBehindRight.canceled -= instance.OnCamBehindRight;
            @Punch.started -= instance.OnPunch;
            @Punch.performed -= instance.OnPunch;
            @Punch.canceled -= instance.OnPunch;
            @Kick.started -= instance.OnKick;
            @Kick.performed -= instance.OnKick;
            @Kick.canceled -= instance.OnKick;
            @CheckMap.started -= instance.OnCheckMap;
            @CheckMap.performed -= instance.OnCheckMap;
            @CheckMap.canceled -= instance.OnCheckMap;
        }

        public void RemoveCallbacks(ICrawlActions instance)
        {
            if (m_Wrapper.m_CrawlActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ICrawlActions instance)
        {
            foreach (var item in m_Wrapper.m_CrawlActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_CrawlActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public CrawlActions @Crawl => new CrawlActions(this);
    public interface ICrawlActions
    {
        void OnLeftArm(InputAction.CallbackContext context);
        void OnRightArm(InputAction.CallbackContext context);
        void OnCamLeft(InputAction.CallbackContext context);
        void OnCamRight(InputAction.CallbackContext context);
        void OnCamBehindLeft(InputAction.CallbackContext context);
        void OnCamBehindRight(InputAction.CallbackContext context);
        void OnPunch(InputAction.CallbackContext context);
        void OnKick(InputAction.CallbackContext context);
        void OnCheckMap(InputAction.CallbackContext context);
    }
}
