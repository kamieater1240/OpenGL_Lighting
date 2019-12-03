#version 330 core

struct Material {
   //vec3 ambient;
   sampler2D diffuse;
   sampler2D specular;
   float shininess;
};

struct Light {
    vec3 position;
	vec3 direction;
	float cutoff;
	float outerCutoff;
	
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
	
	float constant;
	float linear;
	float quadratic;
};

in vec3 Normal;
in vec3 FragPos;
in vec2 MaterialTexCoords;

out vec4 FragColor;

uniform vec3 viewPos;
uniform Material material;
uniform Light spotLight;

void main()
{
	vec3 spotDir = normalize(FragPos - spotLight.position);
	float theta = dot(spotDir, normalize(spotLight.direction));
	float epsilon = spotLight.cutoff - spotLight.outerCutoff;
	float intensity = clamp((theta - spotLight.outerCutoff) / epsilon, 0.0, 1.0);
	
	//Because the ambient of the texture is the same as the diffuse of the texture in most situations
	vec3 ambient = spotLight.ambient * vec3(texture(material.diffuse, MaterialTexCoords));
		
	vec3 norm = normalize(Normal);
	vec3 lightDir = normalize(spotLight.position - FragPos);
	float diff = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = spotLight.diffuse * diff * vec3(texture(material.diffuse, MaterialTexCoords));
		
	vec3 viewDir = normalize(viewPos - FragPos);
	vec3 reflectDir = reflect(-lightDir, norm);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
	vec3 specular = spotLight.specular * (spec * vec3(texture(material.specular, MaterialTexCoords)));
		
	float distance = length(spotLight.position - FragPos);
	float attenuation = 1.0 / (spotLight.constant + spotLight.linear * distance + spotLight.quadratic * distance * distance);
	
	diffuse  *= attenuation;
	specular *= attenuation;
	diffuse  *= intensity;
	specular *= intensity;
		
	vec3 result = ambient + diffuse + specular;
	FragColor = vec4(result, 1.0f);

}