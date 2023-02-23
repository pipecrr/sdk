
namespace Siesa.Global.Enums
{

	public enum MenuType
	{
		Separator = 1,
		Menu = 2,
		Feature = 3
	}

	public enum PermissionUserTypes
	{
		User = 1,
		Team = 2
	}

	public enum PermissionAuthTypes
	{
		Query = 1,
		Transaction = 2,
		Query_Tx = 3
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

	public enum enumDynamicEntityDataType
	{
		Text = 0,
		Number = 1
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
}