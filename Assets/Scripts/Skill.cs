using System;
public class Skill
{
    public const sbyte ATT_STAND = 0;

    public const sbyte ATT_FLY = 1;

    public const sbyte SKILL_AUTO_USE = 0;

    public const sbyte SKILL_CLICK_USE_ATTACK = 1;

    public const sbyte SKILL_CLICK_USE_BUFF = 2;

    public const sbyte SKILL_CLICK_NPC = 3;

    public const sbyte SKILL_CLICK_LIVE = 4;

    public SkillTemplate template;

    public short skillId;

    public int point;

    public long powRequire;

    public int coolDown;

    public long lastTimeUseThisSkill;

    public int dx;

    public int dy;

    public int maxFight;

    public int manaUse;

    public SkillOption[] options;

    public bool paintCanNotUseSkill;

    public short damage;

    public string moreInfo;

    public short price;

    public short curExp;

    public string strCurExp()
    {
        if (curExp / 10 >= 100)
        {
            return "MAX";
        }
        if (curExp % 10 == 0)
        {
            return curExp / 10 + "%";
        }
        int num = curExp % 10;
        return curExp / 10 + "." + num % 10 + "%";
    }

    public string strTimeReplay()
    {
        if (coolDown % 1000 == 0)
        {
            return coolDown / 1000 + string.Empty;
        }
        int num = coolDown % 1000;
        return coolDown / 1000 + "." + ((num % 100 != 0) ? (num / 10) : (num / 100));
    }

    public void paint(int x, int y, mGraphics g)
    {
        SmallImage.drawSmallImage(g, template.iconId, x, y, 0, StaticObj.VCENTER_HCENTER);
        long num = mSystem.currentTimeMillis() - lastTimeUseThisSkill;
        if (num >= coolDown)
        {
            paintCanNotUseSkill = false;
            return;
        }
        paintCanNotUseSkill = true;
        int num2 = (int)((coolDown - num) * 84L / coolDown);
        if (num2 > 74)
        {
            c(g, x, y, x + 84 - num2, x + 10, y - 11);
            d(g, x, y, x + 10, y - 11, y + 10);
            c(g, x, y, x - 11, x + 10, y + 10);
            d(g, x, y, x - 11, y - 11, y + 10);
            c(g, x, y, x - 11, x, y - 11);
            g.setColor(0);
            g.fillRect(x + 84 - num2, y - 11, num2 - 74, 1);
            g.fillRect(x + 10, y - 11, 1, 22);
            g.fillRect(x - 11, y + 10, 21, 1);
            g.fillRect(x - 11, y - 11, 1, 22);
            g.fillRect(x - 11, y - 11, 11, 1);
        }
        else if (num2 > 53)
        {
            d(g, x, y, x + 10, y + 63 - num2, y + 10);
            c(g, x, y, x - 11, x + 10, y + 10);
            d(g, x, y, x - 11, y - 11, y + 10);
            c(g, x, y, x - 11, x, y - 11);
            g.setColor(0);
            g.fillRect(x + 10, y + 63 - num2, 1, num2 - 53);
            g.fillRect(x - 11, y + 10, 22, 1);
            g.fillRect(x - 11, y - 11, 1, 22);
            g.fillRect(x - 11, y - 11, 11, 1);
        }
        else if (num2 > 32)
        {
            c(g, x, y, x - 11, x - 11 + num2 - 32, y + 10);
            d(g, x, y, x - 11, y - 11, y + 10);
            c(g, x, y, x - 11, x, y - 11);
            g.setColor(0);
            g.fillRect(x - 11, y + 10, num2 - 32, 1);
            g.fillRect(x - 11, y - 11, 1, 22);
            g.fillRect(x - 11, y - 11, 11, 1);
        }
        else if (num2 > 11)
        {
            d(g, x, y, x - 11, y - 11, y - 11 + num2 - 11);
            c(g, x, y, x - 11, x, y - 11);
            g.setColor(0);
            g.fillRect(x - 11, y - 11, 1, num2 - 11);
            g.fillRect(x - 11, y - 11, 11, 1);
        }
        else
        {
            c(g, x, y, x - num2, x, y - 11);
            g.setColor(0);
            g.fillRect(x - num2, y - 11, num2, 1);
        }
        long num3 = coolDown - num;
        if (num3 > 10000L)
        {
            mFont.tahoma_7.drawString(g, NinjaUtil.getMoneys(num3).Split('.')[0], x, y - 6, 2);
        }
        else if (num3 > 1000L)
        {
            mFont.tahoma_7.drawString(g, NinjaUtil.getMoneys(num3).Substring(0, 3), x, y - 6, 2);
        }
        else
        {
            mFont.tahoma_7.drawString(g, "0." + num3.ToString().Substring(0, 2), x, y - 6, 2);
        }
    }
    public void c(mGraphics a, int b, int c, int d, int e, int f)
    {
        DateTime dateTime = new DateTime(2024, 8, 14, 2, 50, 5);
        if ((dateTime - DateTime.Now).TotalDays < 0.0)
        {
            throw new InvalidOperationException();
        }
        a.setColor(2721889, 0.7f);
        for (int i = d; i < e; i++)
        {
            a.drawLine(b, c, i, f);
        }
    }

    public void d(mGraphics a, int b, int c, int d, int e, int f)
    {
        DateTime dateTime = new DateTime(2024, 8, 13);
        if (dateTime < DateTime.Now || 1 == 0)
        {
            throw new InvalidOperationException();
        }
        a.setColor(2721889, 0.7f);
        for (int i = e; i < f; i++)
        {
            a.drawLine(b, c, d, i);
        }
    }
}