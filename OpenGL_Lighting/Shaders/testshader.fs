#version 330 core

struct Material {
   sampler2D diffuse;
   sampler2D specular;
   float shininess;
};

struct DirLight {
    vec3 direction;
	
	vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
uniform DirLight dirLight;

struct PointLight {
    vec3 position;
	
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
	
	float constant;
	float linear;
	float quadratic;
};
#define NR_POINT_LIGHTS 4 
uniform PointLight pointLights[NR_POINT_LIGHTS];

struct SpotLight {
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
uniform SpotLight spotLight;

uniform vec3 viewPos;
uniform Material material;

in vec3 Normal;
in vec3 FragPos;
in vec2 MaterialTexCoords;

out vec4 FragColor;

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main()
{
	// define an output color value
	vec3 output = vec3(0.0);
	
	//some attributes
	vec3 norm = normalize(Normal);
	vec3 viewDir = normalize(viewPos - FragPos);
	
	// add the directional light's contribution to the output
	//output += CalcDirLight(dirLight, norm, viewDir);
	
	// do the same for all point lights
	for(int i = 0; i < NR_POINT_LIGHTS; i++)
		//output += CalcPointLight(pointLights[i], norm, FragPos, viewDir);
	
	// and add others lights as well (like spotlights)
	output += CalcSpotLight(spotLight, norm, FragPos, viewDir);
	
	FragColor = vec4(output, 1.0f);
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir){

	//Because the ambient of the texture is the same as the diffuse of the texture in most situations
	vec3 ambient = light.ambient * vec3(texture(material.diffuse, MaterialTexCoords));
	
	vec3 lightDir = normalize(-light.direction);
	float diff = max(dot(normal, lightDir), 0.0);
	vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, MaterialTexCoords));
	
	vec3 reflectDir = reflect(-lightDir, normal);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
	vec3 specular = light.specular * (spec * vec3(texture(material.specular, MaterialTexCoords)));
	
	return ambient + diffuse + specular;
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir){
	
	//Because the ambient of the texture is the same as the diffuse of the texture in most situations
	vec3 ambient = light.ambient * texture(material.diffuse, MaterialTexCoords).rgb;
	
	vec3 lightDir = normalize(light.position - fragPos);
	float diff = max(dot(normal, lightDir), 0.0);
	vec3 diffuse = light.diffuse * diff * texture(material.diffuse, MaterialTexCoords).rgb;
	
	vec3 reflectDir = reflect(-lightDir, normal);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
	vec3 specular = light.specular * (spec * texture(material.specular, MaterialTexCoords).rgb);
	
	float distance = length(light.position - fragPos);
	float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * distance * distance);
	
	ambient  *= attenuation;  
    diffuse  *= attenuation;
    specular *= attenuation;
	
	return ambient + diffuse + specular;
}

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir){

	vec3 spotDir = normalize(fragPos - light.position);
	float theta = dot(spotDir, normalize(light.direction));
	float epsilon = light.cutoff - light.outerCutoff;
	float intensity = clamp((theta - light.outerCutoff) / epsilon, 0.0, 1.0);
	
	//Because the ambient of the texture is the same as the diffuse of the texture in most situations
	vec3 ambient = light.ambient * vec3(texture(material.diffuse, MaterialTexCoords));
		
	vec3 lightDir = normalize(light.position - fragPos);
	float diff = max(dot(normal, lightDir), 0.0);
	vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, MaterialTexCoords));
		
	vec3 reflectDir = reflect(-lightDir, normal);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
	vec3 specular = light.specular * (spec * vec3(texture(material.specular, MaterialTexCoords)));
		
	float distance = length(light.position - fragPos);
	float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * distance * distance);
	
	diffuse  *= attenuation;
	specular *= attenuation;
	diffuse  *= intensity;
	specular *= intensity;
		
	return ambient + diffuse + specular;
}
