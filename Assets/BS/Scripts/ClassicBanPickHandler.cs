using CmrGame;
using CmrGameFramework;
using CmrGameFramework.DataTable;
using CmrGameFramework.Fsm;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSTool
{
    public class ClassicBanPickHandler : BanPickHandlerBase
    {
        private List<CharacterRuntime> m_Pool;
        public List<CharacterRuntime> Pool =>m_Pool;

        public override void OnStart()
        {
            m_Fsm.Start<StateBan>();
            if(IsHost)
            {
                m_Pool = GetCharacters();
            }
            else
            {
                //从主机同步过来
            }

        }
        private List<CharacterRuntime> GetCharacters()
        {
            //获取到全部的英雄表
            IDataTable<DRCharacter> table = GameEntry.DataTable.GetDataTable<DRCharacter>();

            //经典模式下随机选取12个英雄
            List<DRCharacter> results = Utility.Random.GetRandomElements(table, 12).ToList();

            return results.Select((s) =>
            {
                return new CharacterRuntime(s);
            }).ToList();

        }

        public void OnCharacterBanned(List<int> ids)
        {
            foreach(int id in ids)
            {
                m_Pool.Find(s => s.Id == id).IsBanned = true;
            }

        }
    }
}

