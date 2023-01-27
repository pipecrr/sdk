
namespace Siesa.Global.Enums
{

	public enum MenuType
	{
		Separator = 1,
		Submenu = 2,
		Feature = 3,
		CustomMenu= 4
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

	public enum enumYesNo
	{
		Yes = 1,
		No = 0
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
}