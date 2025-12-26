using System.Collections.Generic;
using System;

namespace CmrGameFramework.Fsm
{
    internal class Fsm<T> : FsmBase, IReference, IFsm<T> where T : class
    {
        private T m_Owner;

        private readonly Dictionary<Type, FsmState<T>> m_States;

        private Dictionary<string, Variable> m_Datas;

        private FsmState<T> m_CurrentState;

        private float m_CurrentStateTime;

        private bool m_IsDestroyed;

        public Fsm()
        {
            m_Owner = null;
            m_States = new Dictionary<Type, FsmState<T>>();
            m_Datas = null;
            m_CurrentState = null;
            m_CurrentStateTime = 0f;
            //CYJConfuse：为什么在开始的时候直接设置成true？ 
            m_IsDestroyed = true;
        }

        /// <summary> 获取有限状态机持有者 </summary>
        public T Owner => m_Owner;

        /// <summary> 获取有限状态机持有者类型 </summary>
        public override Type OwnerType => typeof(T);

        /// <summary> 获取有限状态机中状态的数量 </summary>
        public override int FsmStateCount => m_States.Count;

        /// <summary> 获取有限状态机是否正在运行 </summary>
        public override bool IsRunning => m_CurrentState != null;

        /// <summary> 获取有限状态机是否被销毁 </summary>
        public override bool IsDestroyed => m_IsDestroyed;

        /// <summary> 获取当前有限状态机状态 </summary>
        public FsmState<T> CurrentState => m_CurrentState;

        /// <summary> 获取当前有限状态机状态名称 </summary>
        public override string CurrentStateName
        {
            get
            {
                if (m_CurrentState == null)
                {
                    return null;
                }

                return m_CurrentState.GetType().FullName;
            }
        }

        /// <summary> 获取当前有限状态机状态持续时间 </summary>
        public override float CurrentStateTime => m_CurrentStateTime;

        /// <summary>
        /// 创建有限状态机
        /// </summary>
        /// <param name="name"> 有限状态机名称 </param>
        /// <param name="owner"> 有限状态机持有者 </param>
        /// <param name="states"> 有限状态机状态集合 </param>
        /// <returns> 创建的有限状态机 </returns>
        public static Fsm<T> Create(string name, T owner, params FsmState<T>[] states)
        {
            if (owner == null)
                throw Utility.Exception.ThrowException("FSM owner is invalid.");

            if (states == null || states.Length < 1)
                throw Utility.Exception.ThrowException("FSM states is invalid.");

            Fsm<T> fsm = ReferencePool.Acquire<Fsm<T>>();
            fsm.Name = name;
            fsm.m_Owner = owner;
            fsm.m_IsDestroyed = false;
            foreach (FsmState<T> fsmState in states)
            {
                if (fsmState == null)
                    throw Utility.Exception.ThrowException("FSM states is invalid.");

                Type type = fsmState.GetType();
                if (fsm.m_States.ContainsKey(type))
                    throw Utility.Exception.ThrowException(Utility.Text.Format("FSM '{0}' state '{1}' is already exist.",
                        new TypeNamePair(typeof(T), name), type.FullName));

                fsm.m_States.Add(type, fsmState);
                fsmState.OnInit(fsm);
            }
            return fsm;
        }

        /// <summary>
        /// 创建有限状态机
        /// </summary>
        /// <param name="name"> 有限状态机名称 </param>
        /// <param name="owner"> 有限状态机持有者 </param>
        /// <param name="states"> 有限状态机状态集合 </param>
        /// <returns> 创建的有限状态机 </returns>
        public static Fsm<T> Create(string name, T owner, List<FsmState<T>> states)
        {
            if (owner == null)
                throw Utility.Exception.ThrowException("FSM owner is invalid.");

            if (states == null || states.Count < 1)
                throw Utility.Exception.ThrowException("FSM states is invalid.");

            Fsm<T> fsm = ReferencePool.Acquire<Fsm<T>>();
            fsm.Name = name;
            fsm.m_Owner = owner;
            fsm.m_IsDestroyed = false;
            foreach (FsmState<T> state in states)
            {
                if (state == null)
                    throw Utility.Exception.ThrowException("FSM states is invalid.");

                Type type = state.GetType();
                if (fsm.m_States.ContainsKey(type))
                    throw Utility.Exception.ThrowException(Utility.Text.Format("FSM '{0}' state '{1}' is already exist.",
                        new TypeNamePair(typeof(T), name), type.FullName));
                fsm.m_States.Add(type, state);
                state.OnInit(fsm);
            }

            return fsm;
        }

        /// <summary> 清理状态机 </summary>
        public void Clear()
        {
            if (m_CurrentState != null)
                m_CurrentState.OnLeave(this, isShutdown: true);

            foreach (KeyValuePair<Type, FsmState<T>> state in m_States)
                state.Value.OnDestroy(this);

            Name = null;
            m_Owner = null;
            m_States.Clear();
            if (m_Datas != null)
            {
                foreach (KeyValuePair<string, Variable> data in m_Datas)
                    if (data.Value != null)
                        ReferencePool.Release(data.Value);
                m_Datas.Clear();
            }

            m_CurrentState = null;
            m_CurrentStateTime = 0f;
            m_IsDestroyed = true;
        }

        /// <summary>
        /// 开始有限状态机
        /// </summary>
        /// <typeparam name="TState"> 要开始的有限状态机状态类型 </typeparam>
        public void Start<TState>() where TState : FsmState<T>
        {
            if (IsRunning)
                throw Utility.Exception.ThrowException("FSM is running, can not start again.");

            FsmState<T> state = GetState<TState>();
            if (state == null)
                throw Utility.Exception.ThrowException(Utility.Text.Format("FSM '{0}' can not start state '{1}' which is not exist.",
                    new TypeNamePair(typeof(T), Name), typeof(TState).FullName));

            m_CurrentStateTime = 0f;
            m_CurrentState = state;
            m_CurrentState.OnEnter(this);
        }

        /// <summary>
        /// 开始有限状态机
        /// </summary>
        /// <param name="stateType"> 要开始的有限状态机状态类型 </param>
        public void Start(Type stateType)
        {
            if (IsRunning)
                throw Utility.Exception.ThrowException("FSM is running, can not start again.");

            if ((object)stateType == null)
                throw Utility.Exception.ThrowException("State type is invalid.");

            if (!typeof(FsmState<T>).IsAssignableFrom(stateType))
                throw Utility.Exception.ThrowException(Utility.Text.Format("State type '{0}' is invalid.", stateType.FullName));

            FsmState<T> state = GetState(stateType);
            if (state == null)
                throw Utility.Exception.ThrowException(Utility.Text.Format("FSM '{0}' can not start state '{1}' which is not exist.", 
                    new TypeNamePair(typeof(T), Name), stateType.FullName));

            m_CurrentStateTime = 0f;
            m_CurrentState = state;
            m_CurrentState.OnEnter(this);
        }
        /// <summary>
        /// 是否存在有限状态机状态
        /// </summary>
        /// <typeparam name="TState"> 要检查的有限状态机状态类型 </typeparam>
        /// <returns> 是否存在有限状态机状态 </returns>
        public bool HasState<TState>() where TState : FsmState<T>
        {
            return m_States.ContainsKey(typeof(TState));
        }

        /// <summary>
        /// 是否存在有限状态机状态
        /// </summary>
        /// <param name="stateType"> 要检查的有限状态机状态类型 </param>
        /// <returns> 是否存在有限状态机状态 </returns>
        public bool HasState(Type stateType)
        {
            if ((object)stateType == null)
                throw Utility.Exception.ThrowException("State type is invalid.");

            if (!typeof(FsmState<T>).IsAssignableFrom(stateType))
                throw Utility.Exception.ThrowException(Utility.Text.Format("State type '{0}' is invalid.", stateType.FullName));

            return m_States.ContainsKey(stateType);
        }
        /// <summary>
        /// 获取有限状态机状态
        /// </summary>
        /// <typeparam name="TState"> 要获取的有限状态机状态类型 </typeparam>
        /// <returns> 要获取的有限状态机状态 </returns>
        public TState GetState<TState>() where TState : FsmState<T>
        {
            if (m_States.TryGetValue(typeof(TState), out FsmState<T> value))
                return (TState)value;
            return null;
        }

        /// <summary>
        /// 获取有限状态机状态
        /// </summary>
        /// <param name="stateType"> 要获取的有限状态机状态类型 </param>
        /// <returns> 要获取的有限状态机状态 </returns>
        public FsmState<T> GetState(Type stateType)
        {
            if ((object)stateType == null)
                throw Utility.Exception.ThrowException("State type is invalid.");

            if (!typeof(FsmState<T>).IsAssignableFrom(stateType))
                throw Utility.Exception.ThrowException(Utility.Text.Format("State type '{0}' is invalid.", stateType.FullName));

            if (m_States.TryGetValue(stateType, out FsmState<T> value))
                return value;
            return null;
        }

        /// <summary>
        /// 获取有限状态机的所有状态
        /// </summary>
        /// <returns> 有限状态机的所有状态 </returns>
        public FsmState<T>[] GetAllStates()
        {
            int num = 0;
            FsmState<T>[] array = new FsmState<T>[m_States.Count];
            foreach (KeyValuePair<Type, FsmState<T>> state in m_States)
                array[num++] = state.Value;

            return array;
        }

        /// <summary>
        /// 获取有限状态机的所有状态
        /// </summary>
        /// <param name="results"> 有限状态机的所有状态 </param>
        public void GetAllStates(List<FsmState<T>> results)
        {
            if (results == null)
                throw Utility.Exception.ThrowException("Results is invalid.");

            results.Clear();
            foreach (KeyValuePair<Type, FsmState<T>> state in m_States)
                results.Add(state.Value);
        }

        /// <summary>
        /// 是否存在有限状态机数据
        /// </summary>
        /// <param name="name"> 有限状态机数据名称 </param>
        /// <returns> 有限状态机数据是否存在 </returns>
        public bool HasData(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw Utility.Exception.ThrowException("Data name is invalid.");

            if (m_Datas == null)
                return false;

            return m_Datas.ContainsKey(name);
        }

        /// <summary>
        /// 获取有限状态机数据
        /// </summary>
        /// <typeparam name="TData"> 要获取的有限状态机数据的类型 </typeparam>
        /// <param name="name"> 有限状态机数据名称 </param>
        /// <returns> 要获取的有限状态机数据 </returns>
        public TData GetData<TData>(string name) where TData : Variable
        {
            return (TData)GetData(name);
        }

        /// <summary>
        /// 获取有限状态机数据
        /// </summary>
        /// <param name="name"> 有限状态机数据名称 </param>
        /// <returns> 要获取的有限状态机数据 </returns>
        public Variable GetData(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw Utility.Exception.ThrowException("Data name is invalid.");

            if (m_Datas == null)
                return null;

            if (m_Datas.TryGetValue(name, out Variable value))
                return value;

            return null;
        }

        /// <summary>
        /// 设置有限状态机数据
        /// </summary>
        /// <typeparam name="TData"> 有限状态机数据名称 </typeparam>
        /// <param name="name"> 要设置的有限状态机数据 </param>
        /// <param name="data"> 要设置的有限状态机数据的类型 </param>
        public void SetData<TData>(string name, TData data) where TData : Variable
        {
            SetData(name, (Variable)data);
        }

        /// <summary>
        /// 设置有限状态机数据
        /// </summary>
        /// <param name="name"> 有限状态机数据名称 </param>
        /// <param name="data"> 要设置的有限状态机数据 </param>
        public void SetData(string name, Variable data)
        {
            if (string.IsNullOrEmpty(name))
                throw Utility.Exception.ThrowException("Data name is invalid.");

            if (m_Datas == null)
                m_Datas = new Dictionary<string, Variable>(StringComparer.Ordinal);

            Variable data2 = GetData(name);
            if (data2 != null)
                ReferencePool.Release(data2);

            m_Datas[name] = data;
        }

        /// <summary>
        /// 移除有限状态机数据
        /// </summary>
        /// <param name="name"> 有限状态机数据名称 </param>
        /// <returns> 是否移除有限状态机数据成功 </returns>
        public bool RemoveData(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw Utility.Exception.ThrowException("Data name is invalid.");

            if (m_Datas == null)
                return false;

            Variable data = GetData(name);
            if (data != null)
                ReferencePool.Release(data);

            return m_Datas.Remove(name);
        }

        /// <summary>
        /// 有限状态机轮询
        /// </summary>
        /// <param name="elapseSeconds"> 逻辑流逝时间，以秒为单位 </param>
        /// <param name="realElapseSeconds"> 真实流逝时间，以秒为单位 </param>
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_CurrentState != null)
            {
                m_CurrentStateTime += elapseSeconds;
                m_CurrentState.OnUpdate(this, elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary> 关闭并清理有限状态机 </summary>
        internal override void Shutdown()
        {
            ReferencePool.Release(this);
        }
        /// <summary>
        /// 切换当前有限状态机状态
        /// </summary>
        /// <typeparam name="TState"> 要切换到的有限状态机状态类型 </typeparam>

        internal void ChangeState<TState>() where TState : FsmState<T>
        {
            ChangeState(typeof(TState));
        }

        /// <summary>
        /// 切换当前有限状态机状态
        /// </summary>
        /// <param name="stateType"> 要切换到的有限状态机状态类型 </param>
        internal void ChangeState(Type stateType)
        {
            if (m_CurrentState == null)
            {
                throw Utility.Exception.ThrowException("Current state is invalid.");
            }

            FsmState<T> state = GetState(stateType);
            if (state == null)
                throw Utility.Exception.ThrowException(Utility.Text.Format("FSM '{0}' can not change state to '{1}' which is not exist.",
                    new TypeNamePair(typeof(T), base.Name), stateType.FullName));

            m_CurrentState.OnLeave(this, isShutdown: false);
            m_CurrentStateTime = 0f;
            m_CurrentState = state;
            m_CurrentState.OnEnter(this);
        }
    }
}
