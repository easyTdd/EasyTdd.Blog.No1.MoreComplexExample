{{- for namespace in usingNamespaces -}}
using {{namespace}};
{{~end~}}

namespace {{namespace}}
{
	public partial class {{className}}
	{
		public static {{className}} Default()
		{
			return new {{constructorName}}(
			{{~for member in members ~}}
				##Set default {{member.name.camelCase}} value{{if !for.last}},{{end}}
			{{~end~}}
			);
		}
	}
}