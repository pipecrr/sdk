
namespace Siesa.Global.Enums
{

	public enum PermissionUserTypes
	{
		User = 1,
		Team = 2
	}

	public enum PermissionRestrictionType
	{
		Not_Applicable = 0,
		Enabled = 1,
		Disabled = 2
	}

	public enum enumStatusBaseMaster
	{
		Active = 1,
		Inactive = 0
	}

	public enum enumYesNo
	{
		Yes = 1,
		No = 0
	}

	public enum enumAsignPassword
	{
		Automatic = 0,
		Manual = 1
	}

	public enum MenuType
	{
		Separator = 1,
		Submenu = 2,
		Feature = 3,
		CustomMenu = 4
	}

	public enum enumGeneralAction
	{
		None = 0,
		Create = 1,
		Edit = 2,
		Delete = 3,
		List = 4,
		Consult = 5
	}

	public enum enumDynamicEntityDataType
	{
		Text = 0,
		Number = 1,
		Date = 2
	}

	public enum CustomTypeField
	{
		SwitchField = 1,
		SelectBarField = 2,
		TextField = 3,
		EmailField = 4,
		RadioButtonField = 5
	}

	public enum PermissionAuthTypes
	{
		Query = 1,
		Query_Tx = 2
	}

	public enum enumTypeButton
	{
		Create = 1,
		Save = 2,
		Duplicate = 3,
		Edit = 4,
		List = 5,
		Delete = 6
	}
}