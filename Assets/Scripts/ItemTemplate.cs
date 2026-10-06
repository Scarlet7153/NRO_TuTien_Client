public class ItemTemplate
{
	public static bool isShowIdItem;

	public static void initShowIdItem()
	{
		int saved = Rms.loadRMSInt("showIdItem");
		isShowIdItem = (saved == 1);
	}

	public static void toggleShowIdItem()
	{
		isShowIdItem = !isShowIdItem;
		Rms.saveRMSInt("showIdItem", isShowIdItem ? 1 : 0);
		SoundMn.gI().getStrOption();
	}

	public short id;

	public sbyte type;

	public sbyte gender;

	private string _name;

	public string name
	{
		get
		{
			if (isShowIdItem)
			{
				return "[" + id + "] " + _name;
			}
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public string rawName => _name;

	public string[] subName;

	public string description;

	public sbyte level;

	public short iconID;

	public short part;

	public bool isUpToUp;

	public int w;

	public int h;

	public int strRequire;

	public ItemTemplate(short templateID, sbyte type, sbyte gender, string name, string description, sbyte level, int strRequire, short iconID, short part, bool isUpToUp)
	{
		id = templateID;
		this.type = type;
		this.gender = gender;
		_name = name;
		_name = Res.changeString(_name);
		this.description = description;
		this.description = Res.changeString(this.description);
		this.level = level;
		this.strRequire = strRequire;
		this.iconID = iconID;
		this.part = part;
		this.isUpToUp = isUpToUp;
	}
}
