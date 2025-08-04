using RecomeceAPI.Models.Common;
using System.Reflection;

namespace RecomeceAPI.Services.Common
{
  public static class CompareTwoObjectsService
  {
    //string texto = original.CompareEquals(this).ToString() ?? "";
    public static object CompareEquals<T>(this T objectFromCompare, T objectToCompare)
    {
      string ret = "";
      if (objectFromCompare == null && objectToCompare == null)
        return true;

      else if (objectFromCompare == null && objectToCompare != null)
        return false;

      else if (objectFromCompare != null && objectToCompare == null)
        return false;

      PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
      foreach (PropertyInfo prop in props)
      {
        T? objectFromCompare1 = objectFromCompare;
        if (objectFromCompare1 != null)
        {
          object dataFromCompare = objectFromCompare1.GetType()?.GetProperty(prop.Name)?.GetValue(objectFromCompare, null) ?? "";
          if (objectToCompare != null)
          {
            object dataToCompare = objectToCompare.GetType()?.GetProperty(prop.Name)?.GetValue(objectToCompare, null) ?? "";

            if (dataToCompare != null)
            {
              if (!dataFromCompare.Equals(dataToCompare))
                ret += prop.Name + ": " + dataToCompare + "| ";
            }
          }
        }
      }

      return ret;
    }

    public static List<NotificationModel> ValidateRequiredFields<T>(T entity, List<string> requiredProperties, long idRef = 0)
    {
      var notifications = new List<NotificationModel>();

      foreach (var propName in requiredProperties)
      {
        var propInfo = typeof(T).GetProperty(propName);

        if (propInfo != null)
        {
          var value = propInfo.GetValue(entity);
          if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
          {
            notifications.Add(NotificationService.Validation($"Necessário preencher o campo: {propName}"));
          }
        }
        else
        {
          notifications.Add(NotificationService.Validation($"Campo inválido: {propName}"));
        }
      }

      if (notifications.Count == 0)
        notifications.Add(NotificationService.Success("Todos os campos obrigatórios estão preenchidos."));

      return notifications;
    }

  }
}
