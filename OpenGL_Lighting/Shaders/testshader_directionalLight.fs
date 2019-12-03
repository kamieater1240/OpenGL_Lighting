#version 330 core

struct Material {
   //vec3 ambient;
   sampler2D diffuse;
   sampler2D specular;
   float shininess;
};

struct Light {
	// Position is no longer necessary when using directional light
    //vec3 position;
    vec3 direction;
	
	vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

in vec3 Normal;
in vec3 FragPos;
in vec2 MaterialTexCoords;

out vec4 FragColor;

uniform vec3 viewPos;
uniform Material material;
uniform Light dirLight;

void main()
{
	//Because the ambient of the texture is the same as the diffuse of the texture in most situations
	vec3 ambient = dirLight.ambient * vec3(texture(material.diffuse, MaterialTexCoords));
	
	vec3 norm = normalize(Normal);
	vec3 lightDir = normalize(-dirLight.direction);
	float diff = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = dirLight.diffuse * diff * vec3(texture(material.diffuse, MaterialTexCoords));
	
	vec3 viewDir = normalize(viewPos - FragPos);
	vec3 reflectDir = reflect(-lightDir, norm);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
	vec3 specular = dirLight.specular * (spec * vec3(texture(material.specular, MaterialTexCoords)));
	
	vec3 result = ambient + diffuse + specular;
	FragColor = vec4(result, 1.0f);
}