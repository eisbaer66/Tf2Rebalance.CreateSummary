using RtfWriter.Standard;

namespace Tf2Rebalance.CreateSummary
{
    public class RebalanceInfoRtfFormater : RebalanceInfoFormaterIterativeBase
    {
        private RtfDocument _document;

        protected override void Init()
        {
            _document = new RtfDocument(PaperSize.A4, PaperOrientation.Portrait, Lcid.English);
        }

        protected override void WriteCategory(string text)
        {
            RtfParagraph paragraph = _document.addParagraph();
            RtfCharFormat format = paragraph.addCharFormat();
            format.FontStyle.addStyle(FontStyleFlag.Bold | FontStyleFlag.Italic | FontStyleFlag.Underline);
            paragraph.Text.Append(text);
        }

        protected override void WriteClass(string text)
        {
            RtfParagraph paragraph = _document.addParagraph();
            RtfCharFormat format = paragraph.addCharFormat();
            format.FontStyle.addStyle(FontStyleFlag.Bold | FontStyleFlag.Underline);
            paragraph.Text.Append(text);
        }

        protected override void WriteSlot(string text)
        {
            RtfParagraph paragraph = _document.addParagraph();
            RtfCharFormat format = paragraph.addCharFormat();
            format.FontStyle.addStyle(FontStyleFlag.Italic);
            paragraph.Text.Append(text);
        }

        protected override void Write(string weaponnames, Info weapon)
        {
            RtfParagraph paragraphWeaponName = _document.addParagraph();
            RtfCharFormat formatWeaponName = paragraphWeaponName.addCharFormat();
            formatWeaponName.FontStyle.addStyle(FontStyleFlag.Bold);
            paragraphWeaponName.Text.Append(weaponnames);

            RtfParagraph paragraph = _document.addParagraph();
            RtfCharFormat format = paragraph.addCharFormat();
            paragraph.Text.AppendLine(weapon.info);
        }

        protected override string Finalize()
        {
            return _document.render();
        }
    }
}