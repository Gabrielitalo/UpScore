using RecomeceAPI.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Services.Common
{
    public class GenericModelValidatorService
  {
    public List<NotificationModel> GenericValidator<T>(T model)
    {
      List<NotificationModel> list = new List<NotificationModel>();
      if (model == null)
        return list;
      var context = new ValidationContext(model, serviceProvider: null, items: null);
      var results = new List<ValidationResult>();

      bool isValid = Validator.TryValidateObject(model, context, results, true);
      if (!isValid)
      {
        foreach (var validationResult in results)
        {
          list.Add(new NotificationModel
          {
            Type = 3,
            Content = validationResult.ErrorMessage ?? "",
          }
          );
        }
      }
      return list;
    }
  }
}
