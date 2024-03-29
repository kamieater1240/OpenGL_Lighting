#version 330 core

struct Material {
   //vec3 ambient;
   sampler2D diffuse;
   sampler2D specular;
   float shininess;
};

struct Light {
    vec3 position;
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
uniform Light light;

void main()
{
	//Because the ambient of the texture is the same as the diffuse of the texture in most situations
	vec3 ambient = light.ambient * texture(material.diffuse, MaterialTexCoords).rgb;
	
	vec3 norm = normalize(Normal);
	vec3 lightDir = normalize(light.position - FragPos);
	float diff = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = light.diffuse * diff * texture(material.diffuse, MaterialTexCoords).rgb;
	
	vec3 viewDir = normalize(viewPos - FragPos);
	vec3 reflectDir = reflect(-lightDir, norm);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
	vec3 specular = light.specular * (spec * texture(material.specular, MaterialTexCoords).rgb);
	
	float distance = length(light.position - FragPos);
	float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * distance * distance);
	
	ambient  *= attenuation;  
    diffuse   *= attenuation;
    specular *= attenuation;
	
	vec3 result = (ambient + diffuse + specular);
	FragColor = vec4(result, 1.0);
}