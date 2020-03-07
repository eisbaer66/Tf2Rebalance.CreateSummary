using System.Text;

namespace Tf2Rebalance.CreateSummary
{
    public class RebalanceInfoTextFormater : RebalanceInfoFormaterIterativeBase
    {
        private StringBuilder _builder;
        public override string FileExtension => "txt";

        protected override void Init()
        {
            _builder = new StringBuilder();
        }

        protected override void WriteCategory(string text)
        {
            _builder.AppendLine(text);
        }

        protected override void WriteClass(string text)
        {
            _builder.AppendLine(text);
        }

        protected override void WriteSlot(string text)
        {
            _builder.AppendLine(text);
        }

        protected override void Write(string weaponnames, Info weapon)
        {
            _builder.AppendLine(weaponnames);
            _builder.AppendLine(weapon.info);
            _builder.AppendLine();
        }

        protected override string Finalize()
        {
            return _builder.ToString();
        }
    }
}