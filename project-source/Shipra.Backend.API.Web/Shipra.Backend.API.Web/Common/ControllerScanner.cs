using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Web.Common;
	public static class ControllerScanner
	{
		/// <summary>
		/// Get all controllers (API + MVC) and their actions.
		/// </summary>
		public static Dictionary<string, List<string>> GetAllControllersAndActions()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var controllers = assembly.GetTypes()
				.Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract)
				.ToList();

			return BuildControllerDictionary(controllers);
		}

		/// <summary>
		/// Get only MVC (normal) controllers.
		/// </summary>
		public static List<ControllerInfo> GetMvcControllersAndActions()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var controllers = assembly.GetTypes()
				.Where(t => t.IsClass && !t.IsAbstract && typeof(Controller).IsAssignableFrom(t))
				.ToList();

			return BuildControllerInfo(controllers);
		}

		/// <summary>
		/// Get only API controllers (ControllerBase with [ApiController] attribute).
		/// </summary>
		public static Dictionary<string, List<string>> GetApiControllersAndActions()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var controllers = assembly.GetTypes()
				.Where(t => typeof(ControllerBase).IsAssignableFrom(t)
							&& t.GetCustomAttribute<ApiControllerAttribute>() != null
							&& !t.IsAbstract)
				.ToList();

			return BuildControllerDictionary(controllers);
		}

		/// <summary>
		/// Common builder method
		/// </summary>
		private static Dictionary<string, List<string>> BuildControllerDictionary(List<Type> controllers)
		{
			var result = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

			foreach (var controller in controllers)
			{
				var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsSpecialName)
					.Select(m => m.Name)
					.Distinct()
					.ToList();

				var key = controller.Name.Replace("Controller", "");

				if (result.ContainsKey(key))
				{
					// Merge method lists if the controller name already exists
					result[key].AddRange(methods.Where(m => !result[key].Contains(m)));
				}
				else
				{
					result.Add(key, methods);
				}
			}

			return result;
		}
		private static List<ControllerInfo> BuildControllerInfo(List<Type> controllers)
		{
			var result = new List<ControllerInfo>();

			foreach (var controller in controllers)
			{
				var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsSpecialName)
					.Select(m => m.Name)
					.Distinct()
					.ToList();

				var key = controller.Name.Replace("Controller", "");

				var existing = result.FirstOrDefault(r => r.ControllerName.Equals(key, StringComparison.OrdinalIgnoreCase));
				if (existing != null)
				{
					existing.Actions.AddRange(methods.Where(m => !existing.Actions.Contains(m)));
				}
				else
				{
					result.Add(new ControllerInfo
					{
						ControllerName = key,
						Actions = methods
					});
				}
			}

			return result;
		}

	}

	//	// Get all controllers
	//var allControllers = ControllerScanner.GetAllControllersAndActions();

	//		// Get only MVC controllers
	//		var mvcControllers = ControllerScanner.GetMvcControllersAndActions();

	//		// Get only API controllers
	//		var apiControllers = ControllerScanner.GetApiControllersAndActions();

