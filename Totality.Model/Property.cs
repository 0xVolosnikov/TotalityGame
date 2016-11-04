using System.Collections.Generic;


namespace Totality.Model
{
    public class Property
    {
        public double Value { get; set; }
        private List<Buff> _buffs = new List<Buff>();

        public void AddBuff(int time, bool valueType, double buff)
        {
            _buffs.Add(new Buff(time, valueType, buff));
        }

        public double GetBuffedValue()
        {
            var bufValue = Value;
            double mult = 1; //множитель
                             //добавление баффов по значению
            for (int i = 0; i < _buffs.Count; i++)
            {
                if (_buffs[i].ValueType)
                {
                    bufValue += _buffs[i].BuffPower;
                }
                else
                {
                    mult += _buffs[i].BuffPower / 100;
                }
            }

            return bufValue * mult;
        }

        public void NextStep()
        {
            foreach (Buff bff in _buffs)
            {
                bff.Time--;
                if (bff.Time <= 0)
                {
                    _buffs.Remove(bff);
                }
            }
        }

        public class Buff
        {
            public int Time { get; set; }
            public bool ValueType { get; set; } //бафф по значению
            public double BuffPower { get; set; }
            public Buff(int time, bool valueType, double buffPower)
            {
                this.Time = time;
                this.ValueType = valueType;
                this.BuffPower = buffPower;
            }
        }
    }
}
