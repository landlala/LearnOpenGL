# version 330 core

struct Material {
	sampler2D diffuse;
	sampler2D specular;
	float shiness;
};

struct Light {
	vec3 ambient;
	vec3 diffuse;
	vec3 specular;

	vec3 direction;
	vec3 position;

	float constant;
	float linear;
	float quadratic;
	float cutOff;
	float outerCutOff;
};

in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;
out vec4 FragColor;
uniform vec3 viewPos;

uniform Material material;
uniform Light light;

void main()
{
	// attenuation
	// float distance = length(FragPos - light.position);
	// float attenuation = 1 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
	vec3 lightDir = normalize(light.position - FragPos);
	float theta = dot(lightDir, normalize(-light.direction));
	float epsilon   = light.cutOff - light.outerCutOff;
	float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);  

	// ambient
	vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));

	// diffuse
	vec3 norm = normalize(Normal);
	// vec3 lightDir = normalize(-light.direction);
	float diff = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));

	// specular
	vec3 view = normalize(viewPos - FragPos);
	vec3 reflectDir = reflect(-lightDir, norm);
	float spec = pow(max(dot(view, reflectDir), 0.0), material.shiness);
	vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));

	// ambient *= attenuation;
	// diffuse *= attenuation;
	// specular *= attenuation;

	ambient *= intensity;
	diffuse *= intensity;
	specular *= intensity;

	vec3 result = ambient + diffuse + specular;
	FragColor = vec4(result, 1.0f);
}