
namespace Siesa.Global.Enums
{

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

	public enum enumTypeButton
	{
		Create = 1,
		Save = 2,
		Duplicate = 3,
		Edit = 4,
		List = 5,
		Delete = 6
	}

	public enum enumThemeIconStyle
	{
		Solid = 0,
		Regular = 1,
		Light = 2,
		Duotone = 3,
		Thin = 4
	}

	public enum enumThemeTopbarStyle
	{
		OneColor = 0,
		MultiColor = 1
	}

	public enum enumSDKIntegrationStatus
	{
		Queued = 0,
		Processing = 1,
		Success = 2,
		Failed = 3
	}

	public enum enumDynamicEntityDataType
	{
		Text = 0,
		Number = 1,
		Date = 2,
		Time = 3,
		Boolean = 4,
		GenericEntity = 5,
		InternalEntity = 6
	}

	public enum enumGeneralAction
	{
		None = 0,
		Access = 1,
		Create = 2,
		Edit = 3,
		Delete = 4,
		Detail = 5
	}

	public enum EnumPermissionUserTypes
	{
		User = 1,
		Team = 2
	}

	public enum EnumModelMessageType
	{
		Error = 1,
		Info = 2,
		Warning = 3,
	}

	public enum EnumPermissionAuthTypes
	{
		Query = 1,
		Query_Tx = 2
	}

	public enum EnumPermissionRestrictionType
	{
		Not_Applicable = 0,
		Enabled = 1,
		Disabled = 2
	}

	public enum EnumCustomTypeField
	{
		SwitchField = 1,
		SelectBarField = 2,
		TextField = 3,
		EmailField = 4,
		RadioButtonField = 5
	}

	public enum EnumMenuType
	{
		Separator = 1,
		Submenu = 2,
		Feature = 3,
		CustomMenu = 4,
		Suite = 5
	}
}